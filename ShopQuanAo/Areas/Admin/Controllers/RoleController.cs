using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShopQuanAo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: Admin/Role
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        // GET: Admin/Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Role/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var roleExist = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        TempData["Success"] = "Vai trò đã được tạo thành công!";
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Vai trò này đã tồn tại!");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Tên vai trò không được để trống!");
            }

            return View();
        }

        // GET: Admin/Role/Delete/id
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            // Không cho xóa role Admin và User
            if (role.Name == "Admin" || role.Name == "User")
            {
                TempData["Error"] = "Không thể xóa vai trò hệ thống!";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // POST: Admin/Role/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                // Không cho xóa role Admin và User
                if (role.Name == "Admin" || role.Name == "User")
                {
                    TempData["Error"] = "Không thể xóa vai trò hệ thống!";
                    return RedirectToAction(nameof(Index));
                }

                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    TempData["Success"] = "Vai trò đã được xóa thành công!";
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra khi xóa vai trò!";
                }
            }

            return RedirectToAction(nameof(Index));
        }

    }
}



