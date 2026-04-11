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
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
        }

        public async Task<List<Review>> GetByTargetUserIdAsync(string targetUserId)
        {
            return await _context.Reviews
                .Include(r => r.Reviewer)
                .Where(r => r.TargetUserId == targetUserId)
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

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
