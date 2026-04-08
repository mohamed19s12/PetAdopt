using Microsoft.EntityFrameworkCore;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Domain.Entities;
using PetAdopt.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Persistence.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly AppDbContext _context;

        public FavoriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Favorite favorite)
        {
            await _context.Favorites.AddAsync(favorite);
        }

        public async Task<Favorite> GetAsync(string userId, int petId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PetId == petId);
        }

        public async Task<List<Favorite>> GetByUserFavoritesAsync(string userId)
        {
            //user whose added pets
            return await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Pet) 
                .ToListAsync();
        }

        public async Task RemoveAsync(Favorite favorite)
        {
             _context.Favorites.Remove(favorite);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
