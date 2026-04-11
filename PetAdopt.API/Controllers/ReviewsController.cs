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



    }
}
