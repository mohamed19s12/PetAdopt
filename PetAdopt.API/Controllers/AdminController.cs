using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Auth;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPetService _petService;

        public AdminController(UserManager<ApplicationUser> userManager, IPetService petService)
        {
            _userManager = userManager;
            _petService = petService;
        }

        [HttpGet("pending-adopters")]
        public async Task<IActionResult> GetPendingAdopters()
        {
            var pendingUsers = await _userManager.Users.Where(u => !u.IsApproved).ToListAsync();
            var result = pendingUsers.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email
            });
            return Ok(ApiResponse<object>.Success(result, "Fetched pending adopters successfully"));
        }

        [HttpGet("all-adopters")]
        public async Task<IActionResult> GetAllAdopters()
        {
            var adopters = await _userManager.GetUsersInRoleAsync("Adopter");
            var result = adopters.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.IsApproved
            });
            return Ok(ApiResponse<object>.Success(result, "Fetched all adopters successfully"));
        }

        [HttpGet("all-owners")]
        public async Task<IActionResult> GetAllOwners()
        {
            var allOwners = await _userManager.GetUsersInRoleAsync("Adopter");
            var result = allOwners.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.IsApproved
            });
            return Ok(ApiResponse<object>.Success(result, "Fetched all owners successfully"));
        }

        [HttpPut("approve/{userId}")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.Fail("User not found", 404));
            }
            // Approve the user
            user.IsApproved = true;

            //update and check if it succeeded
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<object>.Fail(errors));
            }
            return Ok(ApiResponse<object>.Success(null, "Registered Successfully"));
        }


        [HttpPut("reject/{userId}")]
        public async Task<IActionResult> RejectUser(string userId)
        {
            // Find the user by ID
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound(ApiResponse<object>.Fail("User not found", 404));
            }

            // Not allowed to reject an already approved user
            if (user.IsApproved)
                return BadRequest(ApiResponse<object>.Fail("User is already approved, cannot reject"));

            // Reject the user
            user.IsApproved = false;
            //update and check if it succeeded
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<object>.Fail(errors));
            }
            return Ok(ApiResponse<object>.Success(null, "User rejected successfully"));
        }

        [HttpGet("pending-pets")]
        public async Task<IActionResult> GetPendingPets()
        {
            var result = await _petService.GetPendingAsync();
            return Ok(ApiResponse<List<PetDto>>.Success(result));
        }


        [HttpPut("approve-pet/{petId}")]
        public async Task<IActionResult> ApprovePet(int petId)
        {
            await _petService.ApproveAsync(petId);
            return Ok(ApiResponse<object>.Success(null, "Pet approved successfully"));
        }

        [HttpPut("reject-pet/{petId}")]
        public async Task<IActionResult> RejectPet(int petId)
        {
            await _petService.RejectAsync(petId);
            return Ok(ApiResponse<object>.Success(null, "Pet rejected successfully"));
        }
    }
}
