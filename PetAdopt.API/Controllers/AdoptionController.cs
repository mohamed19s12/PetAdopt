using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.Application.Interfaces.Services;
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

        [HttpPost("{petId}")]
        [Authorize(Roles = "Adopter")]
        public async Task<IActionResult> Apply(int petId)
        {
            //get the user from the token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //if the user is not authorized ... Apply Adoption request
            if (userId == null)
                return Unauthorized();
            await _adoptionService.Apply(userId, petId);
            return Ok("Adoption request submitted successfully.");
        }

        [HttpPost("Accept/{requestId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Accept(int requestId)
        {
            await _adoptionService.Acceept(requestId);
            return Ok("Adoption request accepted.");
        }

        [HttpPost("Reject/{requestId}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Reject(int requestId)
        {
            await _adoptionService.Reject(requestId);
            return Ok("Adoption request rejected.");
        }
    }
}
