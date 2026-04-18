using Microsoft.Extensions.Logging;
using PetAdopt.Application.DTOs.Review;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IReviewRepository reviewRepository, ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
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
            var review = new Review
            {
                ReviewerId = reviewerId,
                TargetUserId = reviewDto.TargetUserId,
                PetId = reviewDto.PetId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment
            };

            await _reviewRepository.AddAsync(review);
            await _reviewRepository.SaveChangesAsync();
            _logger.LogInformation(
                "Review added by User {ReviewerId} for Pet: {PetId}",
                reviewerId, reviewDto.PetId);
        }

        public async Task<List<ReviewDto>> GetReviewsAsync(string targetUserId)
        {
            _logger.LogInformation("Getting reviews for TargetUserId: {TargetUserId}", targetUserId);
            var reviews = await _reviewRepository.GetByTargetUserIdAsync(targetUserId);
            return reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ReviewerName = r.Reviewer.FullName,
                Rating = r.Rating,
                Comment = r.Comment
            }).ToList();
        }
    }
}
