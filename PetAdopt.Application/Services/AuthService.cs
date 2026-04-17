using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PetAdopt.Application.DTOs.Auth;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PetAdopt.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private const int RefreshTokenLifetimeDays = 30;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto, HttpResponse response)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                IsApproved = false
            };
            //create user
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"user registration failed: {errors}");
            }

            var roleName = dto.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
                await _roleManager.CreateAsync(new IdentityRole(roleName));

            await _userManager.AddToRoleAsync(user, roleName);

            return await IssueTokensAsync(user, response);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, HttpResponse response)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new Exception("Invalid email or password");

            if (!user.IsApproved)
                throw new Exception($"Your account is not approved yet. {user.Id}");

            return await IssueTokensAsync(user, response);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(HttpRequest request, HttpResponse response)
        {
            var refreshToken = request.Cookies["refreshToken"];

            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new Exception("Refresh token is missing");

            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresOn <= DateTime.UtcNow)
                throw new Exception("Refresh token is invalid or expired");

            var user = storedToken.User ?? await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
                throw new Exception("User was not found");

            storedToken.IsRevoked = true;
            await _refreshTokenRepository.SaveChangesAsync();

            return await IssueTokensAsync(user, response);
        }

        public async Task LogoutAsync(HttpRequest request, HttpResponse response, string userId)
        {
            var refreshToken = request.Cookies["refreshToken"];

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
                if (storedToken != null)
                {
                    storedToken.IsRevoked = true;
                }
            }

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var userTokens = await _refreshTokenRepository.GetByUserIdAsync(userId);
                foreach (var token in userTokens)
                {
                    token.IsRevoked = true;
                }
            }

            await _refreshTokenRepository.SaveChangesAsync();

            response.Cookies.Delete("refreshToken");
            response.Cookies.Delete("jwt");
        }

        private async Task<AuthResponseDto> IssueTokensAsync(ApplicationUser user, HttpResponse response)
        {
            var accessToken = await _tokenService.CreateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshToken();
            var accessTokenLifetimeMinutes = _configuration.GetValue<int?>("JWT:DurationInMinutes") ?? 1;
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenLifetimeDays);

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresOn = refreshTokenExpiry
            });

            await _refreshTokenRepository.SaveChangesAsync();

            SetAuthCookies(response, accessToken, refreshToken, accessTokenLifetimeMinutes, refreshTokenExpiry);

            return new AuthResponseDto
            {
                Email = user.Email!,
                Token = accessToken
            };
        }

        private static void SetAuthCookies(
            HttpResponse response,
            string accessToken,
            string refreshToken,
            int accessTokenLifetimeMinutes,
            DateTime refreshTokenExpiry)
        {
            response.Cookies.Append("jwt", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(accessTokenLifetimeMinutes)
            });

            response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = refreshTokenExpiry
            });
        }
    }
}
