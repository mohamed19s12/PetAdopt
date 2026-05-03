using Microsoft.EntityFrameworkCore;
using PetAdopt.Application.DTOs.Adoption;
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
    public class AdoptionRequestRepository : IAdoptionRequestRepository
    {
        private readonly AppDbContext _context;

        public AdoptionRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(AdoptionRequest request)
        {
            await _context.AdoptionRequests.AddAsync(request);
        }

        public async Task<AdoptionRequest> GetByIdAsync(int id)
        {
            return await _context.AdoptionRequests
                .Include(a => a.Pet)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public Task<List<AdoptionRequest>> GetByOwnerIdAsync(string ownerId)
        {
            return _context.AdoptionRequests
                .Include(a => a.Pet)
                .Where(a => a.Pet.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<AdoptionRequest>> GetByAdopterIdAsync(string AdopterId , RequestStatus? status = null)
        {
            var query = _context.AdoptionRequests
                .Include(a => a.Pet)
                .Where(a => a.AdoprerId == AdopterId)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(a => a.Status == status.Value);

            return await query.ToListAsync();
        }

        public Task DeleteAsync(AdoptionRequest request)
        {
            _context.AdoptionRequests.Remove(request);
            return Task.CompletedTask;
        }

        public async Task<List<AdoptionRequest>> GetAllRequestsAsync()
        {
            return await _context.AdoptionRequests
                .Include(a => a.Pet)
                    .ThenInclude(p => p.Owner)
                .Include(a => a.Adopter)
                .ToListAsync();
        }

        public async Task<List<AdoptionRequest>> GetAllStatsAsync()
        {
            return await _context.AdoptionRequests.ToListAsync();
        }
    }
}
