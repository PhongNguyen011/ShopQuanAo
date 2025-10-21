using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using ShopQuanAo.Models;
using ShopQuanAo.Models.ViewModels;
using ShopQuanAo.Services;
using System.Text;
using System.Text.Encodings.Web;

namespace ShopQuanAo.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService emailService,
            ILogger<AccountController> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
        }

        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            // Cho phép truy cập Register page ngay cả khi đã đăng nhập
            // User có thể muốn tạo tài khoản mới
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    CreatedDate = DateTime.Now,
                    IsActive = false // Inactive until email confirmed
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    // Assign User role by default
                    await _userManager.AddToRoleAsync(user, "User");

                    // Generate email confirmation token
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    var callbackUrl = Url.Action(
                        "ConfirmEmail",
                        "Account",
                        new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    // Gửi email xác nhận
                    await _emailService.SendEmailAsync(
                        model.Email,
                        "Xác nhận tài khoản của bạn",
                        $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f5f5f5;'>
                                <div style='background-color: white; padding: 30px; border-radius: 10px;'>
                                    <h2 style='color: #333; text-align: center;'>Xác nhận tài khoản</h2>
                                    <p>Xin chào <strong>{model.FullName}</strong>,</p>
                                    <p>Cảm ơn bạn đã đăng ký tài khoản tại Shop Quần Áo.</p>
                                    <p>Vui lòng xác nhận tài khoản của bạn bằng cách nhấp vào nút bên dưới:</p>
                                    <div style='text-align: center; margin: 30px 0;'>
                                        <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}' 
                                           style='background-color: #007bff; color: white; padding: 12px 30px; 
                                                  text-decoration: none; border-radius: 5px; display: inline-block;'>
                                            Xác nhận email
                                        </a>
                                    </div>
                                    <p>Hoặc copy link sau vào trình duyệt:</p>
                                    <p style='word-break: break-all; color: #007bff;'>{callbackUrl}</p>
                                    <p style='color: #666; font-size: 14px;'>
                                        Nếu bạn không đăng ký tài khoản này, vui lòng bỏ qua email này.
                                    </p>
                                </div>
                            </div>
                        </body>
                        </html>
                        ");

                    _logger.LogInformation($"Confirmation link: {callbackUrl}");
                    
                    // Kiểm tra xem email có được cấu hình không
                    var isEmailConfigured = !string.IsNullOrEmpty(_configuration["EmailSettings:SenderEmail"]) &&
                                          _configuration["EmailSettings:SenderEmail"] != "your-email@gmail.com" &&
                                          !string.IsNullOrEmpty(_configuration["EmailSettings:Password"]) &&
                                          _configuration["EmailSettings:Password"] != "your-app-password";
                    
                    if (isEmailConfigured)
                    {
                        TempData["Success"] = "Đăng ký thành công! Vui lòng kiểm tra email để xác nhận tài khoản.";
                    }
                    else
                    {
                        TempData["Warning"] = "Đăng ký thành công! Email chưa được cấu hình. Vui lòng click link bên dưới để xác nhận tài khoản.";
                        TempData["ConfirmLink"] = callbackUrl;
                    }
                    
                    return RedirectToAction("Login");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // Cho phép truy cập Login page ngay cả khi đã đăng nhập
            // User có thể muốn đăng nhập bằng tài khoản khác
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                    return View(model);
                }

                if (!user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng xác nhận email trước khi đăng nhập.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: true);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");

                    // Check if user is admin
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Home", new { area = "Admin" });
                    }

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa tạm thời do đăng nhập sai quá nhiều lần.");
                    return View(model);
                }

                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            }

            return View(model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"Không tìm thấy người dùng với ID: {userId}";
                return View("Error");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                // Activate user account
                user.IsActive = true;
                await _userManager.UpdateAsync(user);

                ViewBag.Message = "Cảm ơn bạn đã xác nhận email. Bây giờ bạn có thể đăng nhập.";
                return View();
            }

            ViewBag.Message = "Xác nhận email thất bại. Link có thể đã hết hạn.";
            return View();
        }

        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Email không tồn tại trong hệ thống.");
                    return View(model);
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Email chưa được xác nhận. Vui lòng xác nhận email trước khi đặt lại mật khẩu.");
                    return View(model);
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                // Không encode token, sử dụng trực tiếp
                // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Action(
                    "ResetPassword",
                    "Account",
                    new { code },
                    protocol: Request.Scheme);

                // Kiểm tra xem email có được cấu hình không
                var isEmailConfigured = !string.IsNullOrEmpty(_configuration["EmailSettings:SenderEmail"]) &&
                                      _configuration["EmailSettings:SenderEmail"] != "your-email@gmail.com" &&
                                      !string.IsNullOrEmpty(_configuration["EmailSettings:Password"]) &&
                                      _configuration["EmailSettings:Password"] != "your-app-password";

                if (isEmailConfigured)
                {
                    try
                    {
                        await _emailService.SendEmailAsync(
                            model.Email,
                            "Đặt lại mật khẩu",
                            $@"
                            <html>
                            <body style='font-family: Arial, sans-serif;'>
                                <div style='max-width: 600px; margin: 0 auto; padding: 20px; background-color: #f5f5f5;'>
                                    <div style='background-color: white; padding: 30px; border-radius: 10px;'>
                                        <h2 style='color: #333; text-align: center;'>Đặt lại mật khẩu</h2>
                                        <p>Xin chào <strong>{user.FullName}</strong>,</p>
                                        <p>Bạn đã yêu cầu đặt lại mật khẩu cho tài khoản của mình.</p>
                                        <p>Vui lòng nhấp vào nút bên dưới để đặt lại mật khẩu:</p>
                                        <div style='text-align: center; margin: 30px 0;'>
                                            <a href='{HtmlEncoder.Default.Encode(callbackUrl!)}' 
                                               style='background-color: #dc3545; color: white; padding: 12px 30px; 
                                                      text-decoration: none; border-radius: 5px; display: inline-block;'>
                                                Đặt lại mật khẩu
                                            </a>
                                        </div>
                                        <p>Hoặc copy link sau vào trình duyệt:</p>
                                        <p style='word-break: break-all; color: #dc3545;'>{callbackUrl}</p>
                                        <p style='color: #666; font-size: 14px;'>
                                            Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.
                                        </p>
                                    </div>
                                </div>
                            </body>
                            </html>
                            ");
                        
                        TempData["Success"] = "Email đặt lại mật khẩu đã được gửi! Vui lòng kiểm tra hộp thư của bạn.";
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending password reset email");
                        TempData["Error"] = "Không thể gửi email. Vui lòng thử lại sau.";
                        return View(model);
                    }
                }
                else
                {
                    TempData["Warning"] = "Email chưa được cấu hình. Vui lòng click link bên dưới để đặt lại mật khẩu.";
                    TempData["ResetLink"] = callbackUrl;
                }

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? code = null)
        {
            if (code == null)
            {
                TempData["Error"] = "Mã xác nhận không hợp lệ.";
                return RedirectToAction("Login");
            }

            try
            {
                // Sử dụng token trực tiếp, không decode
                var model = new ResetPasswordViewModel
                {
                    Code = code,
                    Email = "" // Initialize empty, user will fill it
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error decoding reset password token");
                TempData["Error"] = "Mã xác nhận không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("Login");
            }
        }

        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email không tồn tại trong hệ thống.");
                return View(model);
            }

            try
            {
                _logger.LogInformation($"Attempting to reset password for user: {model.Email}");
                _logger.LogInformation($"Token length: {model.Code?.Length ?? 0}");
                
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Password reset successful for user: {model.Email}");
                    TempData["Success"] = "Mật khẩu đã được đặt lại thành công! Bạn có thể đăng nhập bằng mật khẩu mới.";
                    return RedirectToAction(nameof(ResetPasswordConfirmation));
                }

                _logger.LogWarning($"Password reset failed for user: {model.Email}");
                foreach (var error in result.Errors)
                {
                    _logger.LogWarning($"Reset password error: {error.Code} - {error.Description}");
                    if (error.Code == "InvalidToken")
                    {
                        ModelState.AddModelError(string.Empty, "Mã xác nhận không hợp lệ hoặc đã hết hạn. Vui lòng yêu cầu đặt lại mật khẩu mới.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi đặt lại mật khẩu. Vui lòng thử lại.");
            }

            return View(model);
        }

        // GET: /Account/ResetPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

