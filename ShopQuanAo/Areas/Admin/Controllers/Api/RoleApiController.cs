using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ShopQuanAo.Areas.Admin.Controllers.Api
{
    /// <summary>
    /// API quản lý vai trò (Role) cho Admin.
    /// </summary>
    [Area("Admin")]
    [ApiController]
    [Route("api/admin/roles")]
    [Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class RoleApiController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleApiController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        /// <summary>
        /// Lấy danh sách tất cả các vai trò.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RoleDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            var roles = await _roleManager.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name ?? ""
                })
                .ToListAsync();

            return Ok(roles);
        }

        /// <summary>
        /// Lấy thông tin chi tiết 1 vai trò theo Id.
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDto>> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Id không hợp lệ." });

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Không tìm thấy vai trò." });

            var dto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name ?? ""
            };

            return Ok(dto);
        }

        /// <summary>
        /// Tạo vai trò mới.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RoleDto>> Create([FromBody] RoleCreateUpdateDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(new { message = "Tên vai trò không được để trống!" });

            var roleName = model.Name.Trim();

            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (roleExist)
                return BadRequest(new { message = "Vai trò này đã tồn tại!" });

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Tạo vai trò thất bại.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            var created = await _roleManager.FindByNameAsync(roleName);
            var dto = new RoleDto
            {
                Id = created!.Id,
                Name = created.Name ?? ""
            };

            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        /// <summary>
        /// Cập nhật tên vai trò.
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(RoleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RoleDto>> Update(string id, [FromBody] RoleCreateUpdateDto model)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Id không hợp lệ." });

            if (string.IsNullOrWhiteSpace(model.Name))
                return BadRequest(new { message = "Tên vai trò không được để trống!" });

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Không tìm thấy vai trò." });

            // Không cho sửa tên 2 role hệ thống
            if (role.Name == "Admin" || role.Name == "User")
            {
                return BadRequest(new { message = "Không thể sửa tên vai trò hệ thống (Admin/User)." });
            }

            var newName = model.Name.Trim();

            // Kiểm tra trùng với role khác
            var existing = await _roleManager.FindByNameAsync(newName);
            if (existing != null && existing.Id != role.Id)
            {
                return BadRequest(new { message = "Tên vai trò đã được dùng bởi role khác." });
            }

            role.Name = newName;
            role.NormalizedName = newName.ToUpperInvariant();

            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Cập nhật vai trò thất bại.",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            var dto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name ?? ""
            };

            return Ok(dto);
        }

        /// <summary>
        /// Xoá vai trò theo Id (không cho xoá Admin/User).
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Id không hợp lệ." });

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound(new { message = "Không tìm thấy vai trò." });

            // Không cho xóa role Admin và User
            if (role.Name == "Admin" || role.Name == "User")
            {
                return BadRequest(new { message = "Không thể xóa vai trò hệ thống (Admin/User)." });
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Có lỗi xảy ra khi xóa vai trò!",
                    errors = result.Errors.Select(e => e.Description)
                });
            }

            return NoContent();
        }
    }

    // ============ DTOs dùng cho Swagger / API ============
    public class RoleDto
    {
        /// <summary>Id vai trò.</summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>Tên vai trò.</summary>
        public string Name { get; set; } = string.Empty;
    }

    public class RoleCreateUpdateDto
    {
        /// <summary>Tên vai trò.</summary>
        public string Name { get; set; } = string.Empty;
    }
}
