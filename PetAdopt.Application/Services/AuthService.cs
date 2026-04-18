using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
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
            _logger.LogInformation("Registering new user: {Email}", dto.Email);
            //create user
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("User registration failed: {Errors}", errors);
                throw new Exception($"user registration failed: {errors}");
            }

            var roleName = dto.Role.ToString();
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                _logger.LogInformation("Creating role: {RoleName}", roleName);
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
            await _userManager.AddToRoleAsync(user, roleName);
            _logger.LogInformation("User {Email} registered successfully with role {RoleName}", dto.Email, roleName);
            return await IssueTokensAsync(user, response);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto, HttpResponse response)
        {
            _logger.LogInformation("User login attempt: {Email}", dto.Email);
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", dto.Email);
                throw new Exception("Invalid email or password");
            }
            if (!user.IsApproved)
            {
                _logger.LogWarning("Login attempt for unapproved account: {Email}", dto.Email);
                throw new Exception($"Your account is not approved yet. {user.Id}");
            }
            return await IssueTokensAsync(user, response);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(HttpRequest request, HttpResponse response)
        {
            var refreshToken = request.Cookies["refreshToken"];

            _logger.LogInformation("Refreshing token for refresh token: {RefreshToken}", refreshToken);
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogWarning("Refresh token is missing in the request");
                throw new Exception("Refresh token is missing");
            }

            _logger.LogInformation("Looking up refresh token in the database");
            var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
            if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiresOn <= DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid or expired refresh token: {RefreshToken}", refreshToken);
                throw new Exception("Refresh token is invalid or expired");
            }

            _logger.LogInformation("Refresh token is valid, retrieving associated user");
            var user = storedToken.User ?? await _userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                _logger.LogError("User not found for refresh token: {RefreshToken}", refreshToken);
                throw new Exception("User was not found");
            }
            storedToken.IsRevoked = true;
            await _refreshTokenRepository.SaveChangesAsync();

            _logger.LogInformation("Issuing new tokens for user: {Email} and Revoking old tokens", user.Email);

            return await IssueTokensAsync(user, response);
        }

        public async Task LogoutAsync(HttpRequest request, HttpResponse response, string userId)
        {
            _logger.LogInformation("Logging out user: {UserId}", userId);
            var refreshToken = request.Cookies["refreshToken"];

            _logger.LogInformation("Revoking refresh token: {RefreshToken} for user: {UserId}", refreshToken, userId);
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                _logger.LogInformation("Looking up refresh token in the database for revocation");
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
            _logger.LogInformation("All tokens revoked for user: {UserId}", userId);

            response.Cookies.Delete("refreshToken");
            response.Cookies.Delete("jwt");
        }

        private async Task<AuthResponseDto> IssueTokensAsync(ApplicationUser user, HttpResponse response)
        {
            _logger.LogInformation("Issuing access and refresh tokens for user: {Email}", user.Email);
            var accessToken = await _tokenService.CreateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshToken();

            _logger.LogInformation("Access token and refresh token created for user: {Email}", user.Email);
            //i made it 15 min as default but sometimes i reduce it for testing
            var accessTokenLifetimeMinutes = _configuration.GetValue<int?>("JWT:DurationInMinutes") ?? 15;
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenLifetimeDays);

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresOn = refreshTokenExpiry
            });

            await _refreshTokenRepository.SaveChangesAsync();

            _logger.LogInformation("Refresh token stored in the database for user: {Email} with expiry: {Expiry}", user.Email, refreshTokenExpiry);

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
