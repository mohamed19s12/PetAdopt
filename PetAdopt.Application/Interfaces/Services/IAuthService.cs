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
        Task LogoutAsync(HttpResponse response);
    }
}
