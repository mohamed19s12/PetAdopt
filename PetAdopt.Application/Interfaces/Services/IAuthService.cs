using Microsoft.AspNetCore.Http;
using PetAdopt.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto dto, HttpResponse response);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto, HttpResponse response);
        Task<AuthResponseDto> RefreshTokenAsync(HttpRequest request, HttpResponse response);
        Task LogoutAsync(HttpRequest request, HttpResponse response, string userId);

        //Task<bool> ConfirmEmailAsync(string userId, string token);

        //Task ForgotPasswordAsync(string email);
        //Task ResetPasswordAsync(ResetPasswordDto dto);
    }
}
