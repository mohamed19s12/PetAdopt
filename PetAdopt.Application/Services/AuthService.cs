using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<AuthService> logger,
            IEmailService emailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
            _emailService = emailService;
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

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var confirmationLink = $"{baseUrl}/api/Auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

            _logger.LogInformation("Confirmation email sent to {Email}", user.Email);


            _logger.LogInformation("User {Email} registered successfully with role {RoleName}", dto.Email, roleName);
        
            return new AuthResponseDto
            {
                Email = user.Email
            };
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
            if (!user.EmailConfirmed)
                throw new Exception("Please confirm your email first.");
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

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            //Decode the token 
            var decodedToken = Uri.UnescapeDataString(token);

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
                throw new Exception("Email confirmation failed");

            _logger.LogInformation("Email confirmed for user: {Email}", user.Email);
            return true;
        }

        public async Task ForgotPasswordAsync(string email)
        {
            _logger.LogInformation("Password reset requested for: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || !user.EmailConfirmed)
            {
                _logger.LogWarning("Password reset requested for non-existent or unconfirmed email: {Email}", email);
                return;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var baseUrl = _configuration["AppSettings:BaseUrl"];

            var resetLink = $"{baseUrl}/api/Auth/reset-password?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendResetPasswordEmailAsync(user.Email, resetLink);
            _logger.LogInformation("Password reset email sent to: {Email}", email);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            _logger.LogInformation("Resetting password for userId: {UserId}", dto.UserId);

            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                throw new KeyNotFoundException("User not found");

            //decode token
            var decodedToken = Uri.UnescapeDataString(dto.Token);

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Password reset failed: {Errors}", errors);
                throw new Exception($"Password reset failed: {errors}");
            }

            _logger.LogInformation("Password reset successfully for: {Email}", user.Email);
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
