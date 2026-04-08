using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Repositories
{
    public interface IPetImageRepository
    {
        Task AddAsync(PetImage image);
        Task<List<PetImage>> GetByPetIdAsync(int petId);
        Task SaveChangesAsync();
    }
}
