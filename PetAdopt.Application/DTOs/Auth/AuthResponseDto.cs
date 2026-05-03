using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public UserStatus Status { get; set; }
    }
}
