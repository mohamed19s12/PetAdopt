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
    public class PetImageRepository : IPetImageRepository
    {
        private readonly AppDbContext _context;

        public PetImageRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task AddAsync(PetImage image)
        {
            await _context.PetImages.AddAsync(image);
        }

        public async Task<List<PetImage>> GetByPetIdAsync(int petId)
        {
            return await _context
                .PetImages
                .Where(pi => pi.PetId == petId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
