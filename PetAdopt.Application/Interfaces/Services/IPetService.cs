using PetAdopt.Application.DTOs.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface IPetService
    {
        Task<int> CreateAsync(CreatePetDto dto, string userId);
        Task<List<PetDto>> GetAllAsync();
        Task ApproveAsync(int petId);
    }
}
