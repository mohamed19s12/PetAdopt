using Microsoft.EntityFrameworkCore;
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
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context) { }

        //public async Task AddAsync(Review review)
        //{
        //    await _context.Reviews.AddAsync(review);
        //}

        //public Task DeleteAsync(Review review)
        //{
        //    _context.Reviews.Remove(review);
        //    return Task.CompletedTask;
        //}

        //public async Task<Review> GetByIdAsync(int id)
        //{
        //   return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        //}

        public async Task<List<Review>> GetByPetIdAsync(int petId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.PetId == petId)
                .ToListAsync();
        }

        public async Task<List<Review>> GetByOwnerIdAsync(string ownerId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Include(r => r.Pet)
                    .ThenInclude(p => p.Owner)
                .Where(r => r.Pet.OwnerId == ownerId)
                .ToListAsync();
        }

        // Check if the adopter has adopted any pet from the owner and it's approved
        public async Task<bool> HasAdoptedPetAsync(string adopterId, int petId)
        {
            return await _context.AdoptionRequests
              .AnyAsync(a =>
                    a.AdoprerId == adopterId &&
                    a.Pet.Id == petId &&
                    a.Status == RequestStatus.Approved
                    );
        }

        public async Task<bool> HasReviewedPetAsync(string reviewerId, int petId)
        {
            return await _context.Reviews
              .AnyAsync(r =>
              r.ReviewerId == reviewerId &&
              r.PetId == petId);
        }

        public async Task<List<Review>> GetAllStatsAsync()
        {
            return await _context.Reviews.ToListAsync();
        }

        //public async Task SaveChangesAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}
    }
}
