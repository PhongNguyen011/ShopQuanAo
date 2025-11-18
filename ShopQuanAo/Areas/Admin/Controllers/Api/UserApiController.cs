using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Models;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class UserApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserApiController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>Danh sách user (tóm tắt).</summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.CreatedDate)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FullName,
                    u.PhoneNumber,
                    u.EmailConfirmed,
                    u.IsActive,
                    u.CreatedDate
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>Chi tiết 1 user.</summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUser(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return NotFound();

            return Ok(new
            {
                u.Id,
                u.UserName,
                u.Email,
                u.FullName,
                u.PhoneNumber,
                u.EmailConfirmed,
                u.IsActive,
                u.CreatedDate,
                u.Address,
                u.DateOfBirth,
                u.AvatarFileName
            });
        }
    }
}
