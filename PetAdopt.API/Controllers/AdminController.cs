using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPetService _petService;

        public AdminController(UserManager<ApplicationUser> userManager, IPetService petService)
        {
            _userManager = userManager;
            _petService = petService;
        }

        [HttpGet("approve/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound("User not found");
            }
            // Approve the user
            user.IsApproved = true;

            //update and check if it succeeded
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Failed to approve user: {errors}");
            }
            return Ok("User approved successfully");
        }

        [HttpPost("approve-pet/{petId}")]
        public async Task<IActionResult> ApprovePet(int petId)
        {
            await _petService.ApproveAsync(petId);
            return Ok("Pet approved successfully");
        }
    }
}
