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
        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public async Task AddReviewAsync(string reviewerId, CreateReviewDto reviewDto)
        {
            if(reviewDto.Rating < 1 || reviewDto.Rating > 5)
            {
                throw new ArgumentException("Rating must be between 1 and 5");
            }

            // Check If He Has Adopted The Pet Before Reviewing
            var hasAdopted = await _reviewRepository.HasAdoptedPetAsync(reviewerId, reviewDto.PetId);
            if (!hasAdopted)
                throw new InvalidOperationException("You can only review after a successful adoption");

            // Check If He Has Reviewed The Pet Before
            var hasReviewed = await _reviewRepository.HasReviewedPetAsync(reviewerId, reviewDto.PetId);
            if (hasReviewed)
                throw new InvalidOperationException("You have already reviewed this pet");

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
        }

        public async Task<List<ReviewDto>> GetReviewsAsync(string targetUserId)
        {
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
