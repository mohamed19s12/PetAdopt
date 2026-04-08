using Microsoft.AspNetCore.Identity;
using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Repositories
{
    public interface IPetRepository
    {
        Task<int> AddAsync(Pet pet);
        Task<List<Pet>> GetAllApproovedAsync();
        Task<Pet> GetByIdAsync(int id);
        Task SaveChangesAsync();

    }
}
