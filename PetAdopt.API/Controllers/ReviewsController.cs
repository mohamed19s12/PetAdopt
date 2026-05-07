using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Review;
using PetAdopt.Application.Interfaces.Services;
using System.Security.Claims;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpPost]
        [Authorize(Roles = "Adopter", Policy = "ApprovedOnly")]
        public async Task<IActionResult> AddReview([FromForm] CreateReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _reviewService.AddReviewAsync(userId, dto);
            return Ok(ApiResponse<object>.Success(null, "Review added successfully"));
        }


        [HttpGet("my-reviews")]
        [Authorize(Roles = "Owner", Policy = "ApprovedOnly")]
        public async Task<IActionResult> GetReviewsForOwnerAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _reviewService.GetReviewsForOwnerAsync(userId);
            return Ok(ApiResponse<List<ReviewDto>>.Success(result));
        }

        [HttpGet("i-made")]
        [Authorize(Roles = "Adopter", Policy = "ApprovedOnly")]
        public async Task<IActionResult> GetMyReviews()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var result = await _reviewService.GetReviewsForAdopterAsync(userId);
            return Ok(ApiResponse<List<ReviewDto>>.Success(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Adopter", Policy = "ApprovedOnly")]
        public async Task<IActionResult> UpdateReview(int id, [FromForm] UpdateReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _reviewService.UpdateReviewAsync(userId, id, dto);
            return Ok(ApiResponse<object>.Success(null, "Review updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Adopter", Policy = "ApprovedOnly")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _reviewService.DeleteReviewAsync(userId, id);
            return Ok(ApiResponse<object>.Success(null, "Review deleted successfully"));
        }

        [HttpGet("pet/{petId}")]
        public async Task<IActionResult> GetReviewByPet(int petId)
        {
            var result = await _reviewService.GetReviewByPetIdAsync(petId);

            return Ok(ApiResponse<ReviewDto>.Success(result));
        }



    }
}
