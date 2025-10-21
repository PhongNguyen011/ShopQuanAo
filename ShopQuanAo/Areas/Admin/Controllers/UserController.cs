using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Models;
using ShopQuanAo.Models.ViewModels;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Admin/User
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedDate)
                .ToListAsync();

            var userViewModels = new List<UserManagementViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userViewModels.Add(new UserManagementViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? "",
                    Email = user.Email ?? "",
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    Roles = roles.ToList()
                });
            }

            return View(userViewModels);
        }

        // GET: Admin/User/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email ?? "",
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                SelectedRoles = userRoles.ToList(),
                AllRoles = allRoles.Select(r => new RoleSelectionViewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? "",
                    IsSelected = userRoles.Contains(r.Name ?? "")
                }).ToList()
            };

            return View(model);
        }

        // POST: Admin/User/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Reload roles if validation fails
                var allRoles = await _roleManager.Roles.ToListAsync();
                model.AllRoles = allRoles.Select(r => new RoleSelectionViewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? "",
                    IsSelected = model.SelectedRoles.Contains(r.Name ?? "")
                }).ToList();
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin cơ bản
            user.FullName = model.FullName;
            user.PhoneNumber = model.PhoneNumber;
            user.EmailConfirmed = model.EmailConfirmed;
            user.IsActive = model.IsActive;

            // Cập nhật email nếu thay đổi
            if (user.Email != model.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // Cập nhật username (thường giống email)
                var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);
                if (!setUserNameResult.Succeeded)
                {
                    foreach (var error in setUserNameResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Cập nhật vai trò
            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoleNames = new List<string>();

            // Lấy tên các vai trò được chọn
            foreach (var roleId in model.SelectedRoles)
            {
                var role = await _roleManager.FindByIdAsync(roleId);
                if (role != null && !string.IsNullOrEmpty(role.Name))
                {
                    selectedRoleNames.Add(role.Name);
                }
            }

            // Xóa các vai trò không còn được chọn
            var rolesToRemove = currentRoles.Except(selectedRoleNames).ToList();
            if (rolesToRemove.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                if (!removeResult.Succeeded)
                {
                    foreach (var error in removeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            // Thêm các vai trò mới được chọn
            var rolesToAdd = selectedRoleNames.Except(currentRoles).ToList();
            if (rolesToAdd.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addResult.Succeeded)
                {
                    foreach (var error in addResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }

            TempData["Success"] = "Cập nhật thông tin người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/User/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Không cho xóa chính mình
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "Bạn không thể xóa chính mình!";
                return RedirectToAction(nameof(Index));
            }

            var roles = await _userManager.GetRolesAsync(user);
            var viewModel = new UserManagementViewModel
            {
                Id = user.Id,
                UserName = user.UserName ?? "",
                Email = user.Email ?? "",
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                Roles = roles.ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/User/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Không cho xóa chính mình
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "Bạn không thể xóa chính mình!";
                return RedirectToAction(nameof(Index));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Xóa người dùng thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa người dùng!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/User/ToggleActive/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Không cho khóa chính mình
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser?.Id == user.Id)
            {
                TempData["Error"] = "Bạn không thể khóa chính mình!";
                return RedirectToAction(nameof(Index));
            }

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = user.IsActive 
                    ? "Kích hoạt tài khoản thành công!" 
                    : "Khóa tài khoản thành công!";
            }
            else
            {
                TempData["Error"] = "Có lỗi xảy ra!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

