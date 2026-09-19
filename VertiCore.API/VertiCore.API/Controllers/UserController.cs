using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.DTOs;
using VertiCore.Application.DTOs.User;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Application.Interfaces.Repositories;

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

        [HttpPost("invite")]
        [Authorize(Policy = "TenantAdminOnly")]
        public async Task<IActionResult> InviteUser(InviteUserRequest request)
        {
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
