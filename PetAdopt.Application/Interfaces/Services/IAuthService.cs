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
            Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
            Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}
