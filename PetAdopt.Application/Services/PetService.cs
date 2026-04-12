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
        private readonly INotificationService _notificationService;

        public PetService(IPetRepository petRepository, INotificationService notificationService)
        {
            _petRepository = petRepository;
            _notificationService = notificationService;
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
            if (pet == null)
                throw new Exception("Pet not found");

            //if pet has approved status then not change
            if (pet.Status == PetStatus.Approved)
                throw new Exception("Pet is already approved");

            pet.Status = PetStatus.Approved;
            await _petRepository.SaveChangesAsync();

            //Notify
            await _notificationService.SendNotificationAsync(
                pet.OwnerId, $" Your pet {pet.Name} has been approved and is now visible!");
        }


        public async Task<List<PetDto>> GetAllAsync()
        {
            var pets = await _petRepository.GetAllAsync();
            return  pets.Select(p => new PetDto
            {
                Id = p.Id,
                Name = p.Name,
                Breed = p.Breed,
                Location = p.Location,
                Status = p.Status.ToString()

            }).ToList();
        }

        public async Task<PetDto> GetByIdAsync(int petId)
        {
            var pet = await _petRepository.GetByIdAsync(petId);

            if (pet == null)
                throw new Exception("Pet not found");

            return new PetDto
            {
                Id = pet.Id,
                Name = pet.Name,
                Breed = pet.Breed,
                Location = pet.Location,
                Status = pet.Status.ToString()
            };
        }

        public async Task UpdateAsync(int petId, UpdatePetDto dto, string userId)
        {
            var pet = await _petRepository.GetByIdAsync(petId);

            if (pet == null)
            {
                throw new Exception("Pet Not Found");
            }

            // Check if the user is the owner of the pet
            if (pet.OwnerId != userId)
            {
                throw new Exception("You Not Own This Pet!");
            }

            // Update the pet properties
            pet.Name = dto.Name;
            pet.Age = dto.Age;
            pet.Breed = dto.Breed;
            pet.Gender = dto.Gender;
            pet.HealthStatus = dto.HealthStatus;
            pet.Description = dto.Description;
            pet.Location = dto.Location;

            await _petRepository.UpdateAsync(pet);
            await _petRepository.SaveChangesAsync();

        }

        public async Task DeleteAsync(int petId, string userId)
        {
            
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null)
            {
                throw new Exception("Pet Not Found");
            }
            // Check if the user is the owner of the pet
            if (pet.OwnerId != userId)
            {
                throw new Exception("You Not Own This Pet!");
            }
            await _petRepository.DeleteAsync(petId);
            await _petRepository.SaveChangesAsync();
        }

        public async Task<PageResultDto<PetDto>> SearchAsync(PetFilterDto filter)
        {
            var (pets, totalCount) = await _petRepository.SearchAsync(filter);


            return new PageResultDto<PetDto>
            {
                Items = pets.Select(p => new PetDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Breed = p.Breed,
                    Location = p.Location,
                    Status = p.Status.ToString(),
                    Age = p.Age
                }).ToList() ,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task RejectAsync(int petId)
        {
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null) 
            {
                throw new Exception("Pet not found");
            }

            //if pet has rejected status then not change
            if (pet.Status == PetStatus.Rejected)
                throw new Exception("Pet is already rejected");

            pet.Status = PetStatus.Rejected;
            await _petRepository.SaveChangesAsync();

            //Notify
            await _notificationService.SendNotificationAsync(
                pet.OwnerId, $" Your pet {pet.Name} has been rejected");
        }

        public async Task<List<PetDto>> GetPendingAsync()
        {
            var pendingPets = await _petRepository.GetPendingAsync();
            return pendingPets.Select(p => new PetDto
            {
                Id = p.Id,
                Name = p.Name,
                Breed = p.Breed,
                Location = p.Location,
                Status = p.Status.ToString()
            }).ToList();

        }

        public async Task<List<PetDto>> GetMyPetsAsync(string ownerId)
        {
            var pets = await _petRepository.GetByOwnerIdAsync(ownerId);
            return pets.Select(p => new PetDto
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
