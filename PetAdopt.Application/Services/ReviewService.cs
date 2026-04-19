using AutoMapper;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using PetAdopt.Application.DTOs.Review;
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
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewService> _logger;
        private readonly IDistributedCache _cache;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, ILogger<ReviewService> logger, IDistributedCache cache, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
            _cache = cache;
            _mapper = mapper;
        }

        public async Task AddReviewAsync(string reviewerId, CreateReviewDto reviewDto)
        {
            _logger.LogInformation("Adding review for TargetUserId: {TargetUserId} by ReviewerId: {ReviewerId}", reviewDto.TargetUserId, reviewerId);
            if (reviewDto.Rating < 1 || reviewDto.Rating > 5)
            {
                _logger.LogWarning("Invalid rating: {Rating}", reviewDto.Rating);
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            // Check If He Has Adopted The Pet Before Reviewing
            var hasAdopted = await _reviewRepository.HasAdoptedPetAsync(reviewerId, reviewDto.PetId);
            if (!hasAdopted)
            {
                _logger.LogWarning("User {ReviewerId} has not adopted pet {PetId} and cannot review", reviewerId, reviewDto.PetId);
                throw new InvalidOperationException("You can only review after a successful adoption");
            }
            // Check If He Has Reviewed The Pet Before
            var hasReviewed = await _reviewRepository.HasReviewedPetAsync(reviewerId, reviewDto.PetId);
            if (hasReviewed)
            {
                _logger.LogWarning("User {ReviewerId} has already reviewed pet {PetId}", reviewerId, reviewDto.PetId);
                throw new InvalidOperationException("You have already reviewed this pet");
            }
            var review = _mapper.Map<Review>(reviewDto);
            review.ReviewerId = reviewerId;

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();

            await _cache.RemoveAsync($"reviews_{reviewDto.TargetUserId}");
            _logger.LogInformation(
                "Review added by User {ReviewerId} for Pet: {PetId}",
                reviewerId, reviewDto.PetId);
        }

        public async Task<List<ReviewDto>> GetReviewsAsync(string targetUserId)
        {
            var cacheKey = $"reviews_{targetUserId}";

            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (cachedData != null)
            {
                _logger.LogInformation("Returning reviews for User {UserId} from Redis", targetUserId);
                return JsonSerializer.Deserialize<List<ReviewDto>>(cachedData);
            }

            _logger.LogInformation("Getting reviews for TargetUserId: {TargetUserId}", targetUserId);
            var reviews = await _reviewRepository.GetByTargetUserIdAsync(targetUserId);
            var result = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ReviewerName = r.Reviewer.FullName,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();

            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(result), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            });

            return result;
        }



    }
}
