using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetAdopt.Application.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<PetService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;


        private const string PetsCacheKey = "all_pets";

        public PetService(IPetRepository petRepository, INotificationService notificationService, ILogger<PetService> logger, IDistributedCache cache, IMapper mapper)
        {
            _petRepository = petRepository;
            _notificationService = notificationService;
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task<int> CreateAsync(CreatePetDto dto, string userId)
        {
            _logger.LogInformation("Creating pet: {PetName} for Owner: {UserId}", dto.Name, userId);
            //Mapping CreatePetDto to Pet Entity
            var pet = _mapper.Map<Pet>(dto);
            pet.OwnerId = userId;
            pet.Status = PetStatus.Pending;

            await _petRepository.AddAsync(pet);
            await _petRepository.SaveChangesAsync();

            _logger.LogInformation("Pet created: {PetName} with Id: {PetId}", pet.Name, pet.Id);

            await InvalidateCacheAsync();
            return pet.Id;
        }

        public async Task ApproveAsync(int petId)
        {
            _logger.LogInformation("Approving pet: {PetId}", petId);
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null)
            {
                _logger.LogWarning("Pet not found: {PetId}", petId);
                throw new Exception("Pet not found");
            }
            //if pet has approved status then not change
            if (pet.Status == PetStatus.Approved)
            {
                _logger.LogWarning("Pet already approved: {PetId}", petId);
                throw new Exception("Pet is already approved");
            }
            pet.Status = PetStatus.Approved;
            await _petRepository.SaveChangesAsync();

            _logger.LogInformation("Pet approved: {PetName}", pet.Name);

            //Notify
            await _notificationService.SendNotificationAsync(
                pet.OwnerId, $" Your pet {pet.Name} has been approved and is now visible!");

            await InvalidateCacheAsync(petId);
        }

        public async Task<List<PetDto>> GetAllAsync()
        {
            //Get Data from Cache if exixts
            var cachedData = await _cache.GetStringAsync(PetsCacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Returning pets from Redis cache");
                return JsonSerializer.Deserialize<List<PetDto>>(cachedData);
            }

            _logger.LogInformation("Retrieving all pets");
            var pets = await _petRepository.GetAllAsync();

            //Mapping Pet Entities to PetDto
            var result = _mapper.Map<List<PetDto>>(pets);

            //Store Data in Cache for 5 minutes
            await _cache.SetStringAsync(PetsCacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            _logger.LogInformation("Pets cached in Redis for 5 minutes");
            return result;
        }

        public async Task<PetDto> GetByIdAsync(int petId)
        {
            var cacheKey = $"pet_{petId}";

            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Returning pet {PetId} from Redis cache", petId);
                return JsonSerializer.Deserialize<PetDto>(cachedData);
            }

            _logger.LogInformation("Retrieving pet by Id: {PetId}", petId);
            var pet = await _petRepository.GetByIdAsync(petId);

            if (pet == null)
            {
                _logger.LogWarning("Pet not found: {PetId}", petId);
                throw new Exception("Pet not found");
            }

            var result =  _mapper.Map<PetDto>(pet);

            await _cache.SetStringAsync(cacheKey , JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });
            return result;
        }

        public async Task UpdateAsync(int petId, UpdatePetDto dto, string userId)
        {
            _logger.LogInformation("Updating pet: {PetId} by User: {UserId}", petId, userId);
            var pet = await _petRepository.GetByIdAsync(petId);

            if (pet == null)
            {
                _logger.LogWarning("Pet not found: {PetId}", petId);
                throw new Exception("Pet Not Found");
            }

            // Check if the user is the owner of the pet
            if (pet.OwnerId != userId)
            {
                _logger.LogWarning("User {UserId} is not the owner of pet: {PetId}", userId, petId);
                throw new Exception("You Not Own This Pet!");
            }

            // Update the pet properties
            _mapper.Map(dto, pet);

            await _petRepository.UpdateAsync(pet);
            await _petRepository.SaveChangesAsync();
            _logger.LogInformation("Pet updated: {PetName} (Id: {PetId})", pet.Name, pet.Id);

            await InvalidateCacheAsync(petId);

        }

        public async Task DeleteAsync(int petId, string userId)
        {
            _logger.LogInformation("Deleting pet: {PetId} by User: {UserId}", petId, userId);
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null)
            {
                _logger.LogWarning("Pet not found: {PetId}", petId);
                throw new Exception("Pet Not Found");
            }
            // Check if the user is the owner of the pet
            if (pet.OwnerId != userId)
            {
                _logger.LogWarning("User {UserId} is not the owner of pet: {PetId}", userId, petId);
                throw new Exception("You Not Own This Pet!");
            }
            await _petRepository.DeleteAsync(petId);
            await _petRepository.SaveChangesAsync();
            _logger.LogInformation("Pet deleted: {PetName} (Id: {PetId})", pet.Name, pet.Id);

            await InvalidateCacheAsync(petId);
        }

        public async Task<PageResultDto<PetDto>> SearchAsync(PetFilterDto filter)
        {
            _logger.LogInformation("Searching pets with filter: {@Filter}", filter);
            var (pets, totalCount) = await _petRepository.SearchAsync(filter);

            _logger.LogInformation("Found {PetCount} pets", totalCount);
            return new PageResultDto<PetDto>
            {
                Items = pets.Select(p => _mapper.Map<PetDto>(p)).ToList(),
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task RejectAsync(int petId)
        {
            _logger.LogInformation("Rejecting pet: {PetId}", petId);
            var pet = await _petRepository.GetByIdAsync(petId);
            if (pet == null) 
            {
                _logger.LogWarning("Pet not found: {PetId}", petId);
                throw new Exception("Pet not found");
            }

            //if pet has rejected status then not change
            if (pet.Status == PetStatus.Rejected)
            {
                _logger.LogWarning("Pet already rejected: {PetId}", petId);
                throw new Exception("Pet is already rejected");
            }
            pet.Status = PetStatus.Rejected;
            await _petRepository.SaveChangesAsync();

            _logger.LogInformation("Pet rejected: {PetName}", pet.Name);

            //Notify
            await _notificationService.SendNotificationAsync(
                pet.OwnerId, $" Your pet {pet.Name} has been rejected");

            await InvalidateCacheAsync(petId);
        }

        public async Task<List<PetDto>> GetPendingAsync()
        {
            _logger.LogInformation("Retrieving pending pets for admin panel");
            var pendingPets = await _petRepository.GetPendingAsync();
            _logger.LogInformation("Found {PetCount} pending pets", pendingPets.Count);
            return pendingPets.Select(p => _mapper.Map<PetDto>(p)).ToList();

        }

        public async Task<List<PetDto>> GetMyPetsAsync(string ownerId)
        {
            _logger.LogInformation("Retrieving pets for owner: {OwnerId}", ownerId);
            var pets = await _petRepository.GetByOwnerIdAsync(ownerId);
            _logger.LogInformation("Found {PetCount} pets for owner: {OwnerId}", pets.Count, ownerId);
            return pets.Select(p => _mapper.Map<PetDto>(p)).ToList();
        }


        // Invalidate Cache After Create, Update, Delete, Approve, Reject
        private async Task InvalidateCacheAsync(int? petId = null)
        {
            await _cache.RemoveAsync(PetsCacheKey);

            if (petId.HasValue)
                await _cache.RemoveAsync($"pet_{petId}");

            _logger.LogInformation("Redis cache invalidated");
        }
    }
}
