using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Services;
using System.Security.Claims;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Adopter")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;

        public FavoritesController(IFavoriteService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpPost("petId")]
        public async Task<IActionResult> Add(int petId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();

            await _favoriteService.AddToFavorites(userId, petId);
            return Ok(ApiResponse<object>.Success(null, "Added to favorites"));
        }

        [HttpDelete("petId")]
        public async Task<IActionResult> Remove(int petId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();
            await _favoriteService.RemoveFromFavorites(userId, petId);
            return Ok(ApiResponse<object>.Success(null, "Removed from favorites"));
        }

        [HttpGet]
        public async Task<IActionResult> GetUserFavorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return Unauthorized();
            var result = await _favoriteService.GetUserFavorites(userId);
            return Ok(ApiResponse<List<PetDto>>.Success(result));
        }
    }
}
