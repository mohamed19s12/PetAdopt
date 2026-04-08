using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface IFavoriteService
    {
        Task AddToFavorites(string userId , int petId);
        Task RemoveFromFavorites(string userId, int petId);
        Task<List<PetDto>> GetUserFavorites(string userId);

    }
}
