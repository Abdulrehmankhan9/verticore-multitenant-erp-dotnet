using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.DTOs;
using VertiCore.Application.DTOs.Auth;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Application.Interfaces.Repositories;

namespace VertiCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(result, "Registered successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful"));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _authService.RequestPasswordResetAsync(request);
            return Ok(ApiResponse<string>.Ok(
                "If an account exists for that email, a reset link has been sent.",
                "Password reset requested"));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var reset = await _authService.ResetPasswordAsync(request);
            if (!reset)
                return BadRequest(ApiResponse<string>.Fail("Reset link is invalid or expired"));

            return Ok(ApiResponse<string>.Ok("Password updated", "Password reset successfully"));
        }
    }
}
