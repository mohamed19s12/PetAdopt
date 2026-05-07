using PetAdopt.Application.DTOs.Review;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface IReviewService
    {
        Task AddReviewAsync(string reviewerId, CreateReviewDto review);
        //Task<List<ReviewDto>> GetReviewsAsync(string targetUserId);

        Task UpdateReviewAsync(string userId, int reviewId, UpdateReviewDto dto);
        Task DeleteReviewAsync(string userId, int reviewId);

        Task<List<ReviewDto>> GetReviewsForOwnerAsync(string ownerId);
        Task<ReviewDto> GetReviewByPetIdAsync(int petId);

        Task<List<ReviewDto>> GetReviewsForAdopterAsync(string adopterId);
    }
}
