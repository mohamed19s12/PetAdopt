using Microsoft.AspNetCore.Identity;
using PetAdopt.Application.DTOs.Pet;
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
        Task<List<Pet>> GetAllAsync();
        Task<Pet> GetByIdAsync(int id);

        Task UpdateAsync(Pet pet);
        Task DeleteAsync(int id);

        Task SaveChangesAsync();

        // Filtering and Searching
        Task<(List<Pet> Pets, int totalCount)> SearchAsync(PetFilterDto filter);

        //Getiing Pending Pets For Admin Panel
        Task<List<Pet>> GetPendingAsync();

        //Get Owner pets for each user
        Task<List<Pet>> GetByOwnerIdAsync(string ownerId);
    }
}
