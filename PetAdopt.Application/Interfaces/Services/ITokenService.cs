using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(ApplicationUser user);
        Task<string> CreateRefreshToken();
    }
}
