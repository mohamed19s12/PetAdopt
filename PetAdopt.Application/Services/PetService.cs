using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;

        public PetService(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        public async Task<int> CreateAsync(CreatePetDto dto, string userId)
        {
            //Mapping CreatePetDto to Pet Entity
            var pet = new Pet
            {
                Name = dto.Name,
                Age = dto.Age,
                Breed = dto.Breed,
                Gender = dto.Gender,
                HealthStatus = dto.HealthStatus,
                Description = dto.Description,
                Location = dto.Location,
                OwnerId = userId,
                Status = PetStatus.Pending
            };

            await _petRepository.AddAsync(pet);
            await _petRepository.SaveChangesAsync();

            return pet.Id;
        }

        public async Task ApproveAsync(int petId)
        {
            var pet = await _petRepository.GetByIdAsync(petId);

            //if pet is null, throw an exception else update its status to approved
            if (pet == null)
                throw new Exception("Pet not found");

            pet.Status = PetStatus.Approved;
            await _petRepository.SaveChangesAsync();
        }


        public async Task<List<PetDto>> GetAllAsync()
        {
            var pets = await _petRepository.GetAllApproovedAsync();
            return  pets.Select(p => new PetDto
            {
                Id = p.Id,
                Name = p.Name,
                Breed = p.Breed,
                Location = p.Location,
                Status = p.Status.ToString()

            }).ToList();
        }

    }
}
