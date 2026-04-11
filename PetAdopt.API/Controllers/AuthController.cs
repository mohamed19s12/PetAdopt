using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Auth;
using PetAdopt.Application.Interfaces.Services;

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
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Registered Successfully"));
        }

        [HttpPost("login")]
        [EnableRateLimiting("Auth-Limit")]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto ,Response);
            return Ok(ApiResponse<AuthResponseDto>.Success(result, "Logged in Successfully"));
        }


        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync(Response);
            return Ok(ApiResponse<object>.Success("Logged out successfully"));
        }


    }
}
