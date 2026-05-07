using Microsoft.EntityFrameworkCore;
using PetAdopt.Application.DTOs.Favorite;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using PetAdopt.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Persistence.Repositories
{
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(AppDbContext context) : base(context) { }

        //public async Task AddAsync(Favorite favorite)
        //{
        //    await _context.Favorites.AddAsync(favorite);
        //}

        public async Task<Favorite> GetAsync(string userId, int petId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.PetId == petId);
        }

        public async Task<List<PetWithFavoriteDto>> GetAllPetsWithFavoriteAsync(string userId)
        {
            var favoritePetIds = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Select(f => f.PetId)
                .ToListAsync();

            var pets = await _context.Pets
                .Include(p => p.Owner)
                    .ThenInclude(o => o.ReviewsReceived)
                .Include(p => p.Images)
                .Where(p => p.postsApprovalStatus == PostsApprovalStatus.Approved)
                .ToListAsync();

            return pets.Select(p => new PetWithFavoriteDto
            {
                Id = p.Id,
                Name = p.Name,
                Breed = p.Breed,
                Gender = p.Gender,
                Description = p.Description,
                Location = p.Location,
                HealthStatus = p.HealthStatus,
                Age = p.Age,

                Images = p.Images?.Select(i => i.ImageUrl).ToList(),

                PetStatusForAdoption = p.petStatusForAdoption.ToString(),
                PostsApprovalStatus = p.postsApprovalStatus.ToString(),

                OwnerName = p.Owner?.FullName,
                OwnerRating = p.Owner?.ReviewsReceived?.Any() == true
                    ? (int)p.Owner.ReviewsReceived.Average(r => r.Rating)
                    : 0,

                IsFavorite = favoritePetIds.Contains(p.Id)
            }).ToList();
        }
        //public async Task RemoveAsync(Favorite favorite)
        //{
        //     _context.Favorites.Remove(favorite);
        //}

        //public async Task SaveChangesAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}
    }
}
