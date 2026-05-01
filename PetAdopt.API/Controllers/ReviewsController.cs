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
        [Authorize(Roles = "Adopter")]
        public async Task<IActionResult> AddReview([FromForm] CreateReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _reviewService.AddReviewAsync(userId, dto);
            return Ok(ApiResponse<object>.Success(null, "Review added successfully"));
        }

        [HttpGet("{targetUserId}")]
        public async Task<IActionResult> GetReviews(string targetUserId)
        {
            var result = await _reviewService.GetReviewsAsync(targetUserId);
            return Ok(ApiResponse<List<ReviewDto>>.Success(result));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Adopter")]
        public async Task<IActionResult> UpdateReview(int id, [FromForm] UpdateReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _reviewService.UpdateReviewAsync(userId, id, dto);
            return Ok(ApiResponse<object>.Success(null, "Review updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Adopter")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            await _reviewService.DeleteReviewAsync(userId, id);
            return Ok(ApiResponse<object>.Success(null, "Review deleted successfully"));
        }



    }
}
