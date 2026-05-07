using PetAdopt.Application.DTOs.Favorite;
using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Repositories
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        //Adding Favorite Pet to User's Favorite List
        //Task AddAsync(Favorite favorite);

        //Removing Favorite Pet from User's Favorite List
        //Task RemoveAsync(Favorite favorite);

        //Getting User's Favorite List
        Task<List<PetWithFavoriteDto>> GetAllPetsWithFavoriteAsync(string userId);

        //Getting Specific Favorite Pet from User's Favorite List
        Task<Favorite> GetAsync(string userId, int petId);

        // Saving Changes to the Database
        //Task SaveChangesAsync();

    }
}
