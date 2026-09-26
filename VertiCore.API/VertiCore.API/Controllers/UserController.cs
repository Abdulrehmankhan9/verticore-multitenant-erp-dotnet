using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.DTOs;
using VertiCore.Application.DTOs.User;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Domain.Enums;

namespace VertiCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(Policy = "TenantAdminOnly")]
        public async Task<IActionResult> GetUsers()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            var tenantId = Guid.Parse(tenantIdClaim!);

            var users = await _userService.GetUsersAsync(tenantId);
            return Ok(ApiResponse<List<UserDto>>.Ok(users, "Users fetched"));
        }

        [HttpPost("invite")]
        [Authorize(Policy = "TenantAdminOnly")]
        public async Task<IActionResult> InviteUser(InviteUserRequest request)
        {
            if (request.Role is not (UserRole.Manager or UserRole.Staff))
                return BadRequest(ApiResponse<string>.Fail("This role cannot be assigned by a tenant administrator"));

            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            var tenantId = Guid.Parse(tenantIdClaim!);

            await _userService.InviteUserAsync(request, tenantId);
            return Ok(ApiResponse<string>.Ok("Invited", "User invited successfully"));
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword(SetPasswordRequest request)
        {
            await _userService.SetPasswordAsync(request);
            return Ok(ApiResponse<string>.Ok("Done", "Password set successfully. You can now login."));
        }
    }
}