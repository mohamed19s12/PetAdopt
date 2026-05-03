using Microsoft.EntityFrameworkCore;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using PetAdopt.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PetAdopt.Persistence.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Pet pet)
        {
            _context.Pets.Add(pet);
        }

        public Task DeleteAsync(int id)
        {
            var pet = _context.Pets.Find(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                return _context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Pet not found");
            }
        }

        public async Task<List<Pet>> GetAllAsync()
        {
            return await _context.Pets
                .Where(p => p.Status == PetStatus.Approved || p.Status == PetStatus.Adopted)
                
                .ToListAsync();
            
        }

        public async Task<Pet> GetByIdAsync(int id)
        {

            return await _context.Pets
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public Task<List<Pet>> GetPendingAsync()
        {
            var pendingPets = _context.Pets
                .Where(p => p.Status == PetStatus.Pending)
                .ToListAsync();
            return pendingPets;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<(List<Pet> Pets , int totalCount)> SearchAsync(PetFilterDto filter)
        {
            // Start with all approved or adopted pets
            var query = _context.Pets
                .Where(p => p.Status == PetStatus.Approved || p.Status == PetStatus.Adopted)
                .AsQueryable();
            // Apply search term filter For String Properties
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var term = $"%{filter.SearchTerm.ToLower()}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Name.ToLower(), term) ||
                    EF.Functions.Like(p.Breed.ToLower(), term) ||
                    EF.Functions.Like(p.Description.ToLower(), term)
                );

            }
            // Apply age filter
            if (filter.Age.HasValue)
                query = query.Where(p => p.Age >= filter.Age.Value);

            //Sort
          
            query = filter.SortBy switch
            {
                SortBy.Name => filter.IsDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
                SortBy.Age => filter.IsDescending ? query.OrderByDescending(p => p.Age) : query.OrderBy(p => p.Age),
                _ => query.OrderBy(p => p.Id)
            };
            

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            //Pagination
            var pets = await query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            return (pets, totalCount);

        }

        public async Task UpdateAsync(Pet pet)
        {
             _context.Pets.Update(pet);
        }

        public async Task<List<Pet>> GetByOwnerIdAsync(string ownerId)
        {
            return await _context.Pets
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<List<Pet>> GetAllStatsAsync()
        {
            return await _context.Pets.ToListAsync();
        }
    }
}
