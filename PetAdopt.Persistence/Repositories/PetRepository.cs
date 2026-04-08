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
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Pet pet)
        {
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
            return pet.Id;

        }

        public async Task<List<Pet>> GetAllApproovedAsync()
        {
            return await _context.Pets.Where(p => p.Status == PetStatus.Approved).ToListAsync();
            
        }

        public async Task<Pet> GetByIdAsync(int id)
        {
            return await _context.Pets.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
