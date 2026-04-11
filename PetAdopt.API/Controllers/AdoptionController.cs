using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Adoption;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Enums;
using System.Security.Claims;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoptionController : ControllerBase
    {
        private readonly IAdoptionService _adoptionService;

        public AdoptionController(IAdoptionService adoptionService)
        {
            _adoptionService = adoptionService;
        }

        [HttpPost("request-pet-for-adoption/{petId}")]
        [Authorize(Roles = "Adopter")]
        public async Task<IActionResult> Apply(int petId)
        {
            //get the user from the token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if the user is not authorized ... Apply Adoption request
            if (userId == null)
                return Unauthorized();
            await _adoptionService.Apply(userId, petId);
            return Ok(ApiResponse<object>.Success(null, "Adoption request submitted successfully"));
        }

        [HttpPut("Accept-adoption-request/{requestId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Accept(int requestId)
        {
            await _adoptionService.Acceept(requestId);
            return Ok(ApiResponse<object>.Success(null, "Adoption request accepted"));
        }

        [HttpPut("Reject-adoption-request/{requestId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Reject(int requestId)
        {
            await _adoptionService.Reject(requestId);
            return Ok(ApiResponse<object>.Success(null, "Adoption request rejected"));
        }

        [HttpGet("my-requests")]
        [Authorize(Roles = "Adopter")]
        public async Task<IActionResult> GetMyRequests([FromQuery] RequestStatus? status)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var result = await _adoptionService.GetMyRequestsAsync(userId, status);
            return Ok(ApiResponse<List<AdoptionRequestDto>>.Success(result));
        }
    }
}
