using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Repositories
{
    public interface IFavoriteRepository
    {
        //Adding Favorite Pet to User's Favorite List
        Task AddAsync(Favorite favorite);
        
        //Removing Favorite Pet from User's Favorite List
        Task RemoveAsync(Favorite favorite);
        
        //Getting User's Favorite List
        Task<List<Favorite>> GetByUserFavoritesAsync(string userId);
        
        //Getting Specific Favorite Pet from User's Favorite List
        Task<Favorite> GetAsync(string userId, int petId);

        // Saving Changes to the Database
        Task SaveChangesAsync();

    }
}
