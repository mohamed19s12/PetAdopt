using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PetAdopt.Application.Services
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ILogger<FavoriteService> _logger;
        private readonly IDistributedCache _cache;

        public FavoriteService(IFavoriteRepository favoriteRepository, ILogger<FavoriteService> logger, IDistributedCache cache)
        {
            _favoriteRepository = favoriteRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task AddToFavorites(string userId, int petId)
        {
            _logger.LogInformation("Adding pet: {PetId} to favorites for user: {UserId}", petId, userId);
            //First we Checking if the pet is already in the user's favorites
            var exists = await _favoriteRepository.GetAsync(userId, petId);

            if (exists != null)
            {
                _logger.LogWarning("Pet: {PetId} is already in favorites for user: {UserId}", petId, userId);
                throw new InvalidOperationException("This pet is already in your favorites.");
            }
            //check if petId is valid
            if (petId == null)
                throw new Exception("This pet not exists");

            //IS NOT EXISTS
            var favorite = new Favorite 
            { 
                UserId = userId,
                PetId = petId
            };

            await _favoriteRepository.AddAsync(favorite);
            await _favoriteRepository.SaveChangesAsync();

            await _cache.RemoveAsync($"favorites_{userId}");
            _logger.LogInformation("Pet: {PetId} added to favorites for user: {UserId} , cache invalidated", petId, userId);
        }

        public async Task<List<PetDto>> GetUserFavorites(string userId)
        {
            var cacheKey = $"favorites_{userId}";

            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Returning favorites for User {UserId} from Redis", userId);
                return JsonSerializer.Deserialize<List<PetDto>>(cachedData);
            }

            _logger.LogInformation("Retrieving favorites for user: {UserId}", userId);
            var favorites = await _favoriteRepository.GetByUserFavoritesAsync(userId);

            //f => Each Favorite that i Take Pet from it
            var result = favorites.Select(f => new PetDto
            {
                Id = f.Pet.Id,
                Name = f.Pet.Name,
                Breed = f.Pet.Breed,
                Location = f.Pet.Location,
                Status = f.Pet.Status.ToString()
            }).ToList();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return result;

        }

        public async Task RemoveFromFavorites(string userId, int petId)
        {
            _logger.LogInformation("Removing pet: {PetId} from favorites for user: {UserId}", petId, userId);
            //Catch the pet that i want to remove
            var favorite = await _favoriteRepository.GetAsync(userId, petId);

            if (favorite == null)
            {
                _logger.LogWarning("Pet: {PetId} not found in favorites for user: {UserId}", petId, userId);
                throw new InvalidOperationException("Not Found.");
            }

            //remove it if exists and save changes
            await _favoriteRepository.RemoveAsync(favorite);
            await _favoriteRepository.SaveChangesAsync();

            await _cache.RemoveAsync($"favorites_{userId}");
            _logger.LogInformation("Pet: {PetId} removed from favorites for user: {UserId} , cache invalidated", petId, userId);
        }
    }
}
