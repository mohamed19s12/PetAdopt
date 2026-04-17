using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Auth;
using PetAdopt.Application.Interfaces.Services;
using System.Security.Claims;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [EnableRateLimiting("Auth-Limit")]
        public async Task<IActionResult> Register([FromForm] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto, Response);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Registered Successfully , please wait admin to aprove your account!"));
        }

        [HttpPost("login")]
        [EnableRateLimiting("Auth-Limit")]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto ,Response);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Logged in Successfully"));
        }

        [HttpPost("refresh")]
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


    }
}
