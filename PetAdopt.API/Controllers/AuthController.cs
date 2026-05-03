using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Auth;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using System.Security.Claims;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost("register")]
        [EnableRateLimiting("Auth-Limit")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto, Response);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Please Wait For Admin Approval"));
        }

        [HttpPost("login")]
        [EnableRateLimiting("Auth-Limit")]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto ,Response);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Logged in Successfully"));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> Refresh()
        {
            var result = await _authService.RefreshTokenAsync(Request, Response);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Token refreshed successfully"));
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _authService.LogoutAsync(Request, Response, userId!);
            return Ok(ApiResponse<object>.Success("Logged out successfully"));
        }

        //[HttpGet("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        //{
        //    await _authService.ConfirmEmailAsync(userId, token);
        //    return Ok(ApiResponse<object>.Success(null, "Email confirmed successfully!"));
        //}

        //[HttpPost("forgot-password")]
        //[EnableRateLimiting("Auth-Limit")]
        //public async Task<IActionResult> ForgotPassword([FromForm] string email)
        //{
        //    await _authService.ForgotPasswordAsync(email);
        //    // return the same response regardless of whether the email exists to prevent user enumeration
        //    return Ok(ApiResponse<object>.Success(null,
        //        "If this email exists, you will receive a reset link."));
        //}

        //[HttpPost("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto dto)
        //{
        //    await _authService.ResetPasswordAsync(dto);
        //    return Ok(ApiResponse<object>.Success(null, "Password reset successfully!"));
        //}


        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(ApiResponse<object>.Success(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Status,
                Role = roles.FirstOrDefault()
            }));
        }

    }
}
