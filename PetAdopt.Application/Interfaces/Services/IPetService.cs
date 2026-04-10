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

        Task<PetDto> GetByIdAsync(int petId);
        //getting pets by user id then mapping them to PetDto
        Task UpdateAsync(int petId, UpdatePetDto dto , string userId);
        Task DeleteAsync(int petId , string userId);

        Task ApproveAsync(int petId);

        //Applying Filters and Search
        Task<PageResultDto<PetDto>> SearchAsync(PetFilterDto filter);
    }
}
