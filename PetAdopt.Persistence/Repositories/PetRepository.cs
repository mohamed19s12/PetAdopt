using Microsoft.EntityFrameworkCore;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using PetAdopt.Persistence.Context;

namespace PetAdopt.Persistence.Repositories
{
    public class PetRepository : GenericRepository<Pet>, IPetRepository
    {
        public PetRepository(AppDbContext context) : base(context) { }

        // -------------------------
        // CREATE
        // -------------------------
        //public async Task AddAsync(Pet pet)
        //{
        //    await _context.Pets.AddAsync(pet);
        //}

        // -------------------------
        // DELETE (FIXED)
        // -------------------------
        public async Task DeleteAsync(int id)
        {
            var pet = await _context.Pets
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pet == null)
                throw new Exception("Pet not found");

            // delete images first (important)
            if (pet.Images != null && pet.Images.Any())
            {
                _context.PetImages.RemoveRange(pet.Images);
            }

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync();
        }

        // -------------------------
        // GET ALL (PUBLIC)
        // -------------------------
        public async Task<List<Pet>> GetAllAsync()
        {
            return await _context.Pets
                .Include(p => p.Images)
                .Include(p => p.Owner)
                .Where(p => p.Status == PetStatus.Approved
                         || p.Status == PetStatus.Adopted)
                .ToListAsync();
        }

        // -------------------------
        // GET BY ID
        // -------------------------
        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _context.Pets
                .Include(p => p.Images)
                .Include(p => p.Owner)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        // -------------------------
        // GET PENDING
        // -------------------------
        public async Task<List<Pet>> GetPendingAsync()
        {
            return await _context.Pets
                .Include(p => p.Images)
                .Where(p => p.Status == PetStatus.Pending)
                .ToListAsync();
        }

        // -------------------------
        // GET BY OWNER
        // -------------------------
        public async Task<List<Pet>> GetByOwnerIdAsync(string ownerId)
        {
            return await _context.Pets
                .Include(p => p.Images)
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        // -------------------------
        // UPDATE
        // -------------------------
        //public Task UpdateAsync(Pet pet)
        //{
        //    _context.Pets.Update(pet);
        //    return Task.CompletedTask;
        //}

        // -------------------------
        // SAVE
        // -------------------------
        //public async Task SaveChangesAsync()
        //{
        //    await _context.SaveChangesAsync();
        //}

        // -------------------------
        // SEARCH (FIXED & CLEAN)
        // -------------------------
        public async Task<(List<Pet> Pets, int totalCount)> SearchAsync(PetFilterDto filter)
        {
            var query = _context.Pets
                .Include(p => p.Images)
                .Where(p => p.Status == PetStatus.Approved
                         || p.Status == PetStatus.Adopted)
                .AsQueryable();

            // SEARCH
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.ToLower();

                query = query.Where(p =>
                    p.Name.ToLower().Contains(term) ||
                    p.Breed.ToLower().Contains(term) ||
                    p.Description.ToLower().Contains(term));
            }

            // FILTER AGE
            if (filter.Age.HasValue)
                query = query.Where(p => p.Age >= filter.Age.Value);

            // SORT
            query = filter.SortBy switch
            {
                SortBy.Name => filter.IsDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),

                SortBy.Age => filter.IsDescending
                    ? query.OrderByDescending(p => p.Age)
                    : query.OrderBy(p => p.Age),

                _ => query.OrderBy(p => p.Id)
            };

            var totalCount = await query.CountAsync();

            var pets = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (pets, totalCount);
        }

        // -------------------------
        // STATS
        // -------------------------
        public async Task<List<Pet>> GetAllStatsAsync()
        {
            return await _context.Pets
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task DeleteImagesByPetIdAsync(int petId)
        {
            var images = await _context.PetImages
                .Where(i => i.PetId == petId)
                .ToListAsync();

            _context.PetImages.RemoveRange(images);
        }

        public async Task<List<Pet>> GetApprovedAsync()
        {
             return await _context.Pets
            .Include(p => p.Images)
            .Where(p => p.Status == PetStatus.Approved)
            .ToListAsync();
        }

        public async Task<List<Pet>> GetRejectedAsync()
        {
            return await _context.Pets
                .Include(p => p.Images)
                .Where(p => p.Status == PetStatus.Rejected)
                .ToListAsync();
        }
    }
}