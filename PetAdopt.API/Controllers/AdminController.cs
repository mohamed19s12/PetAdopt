using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Auth;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Application.Services;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPetService _petService;
        private readonly IDashboardService _dashboardService;


        public AdminController(UserManager<ApplicationUser> userManager, IPetService petService, IDashboardService dashboardService)
        {
            _userManager = userManager;
            _petService = petService;
            _dashboardService = dashboardService;
        }

        [HttpDelete("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user =await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound(ApiResponse<object>.Fail("User not found", 404));

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(ApiResponse<object>.Fail(errors));
            }

            return Ok(ApiResponse<object>.Success(null, "User deleted successfully"));
        }

        [HttpGet("pending-users")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var pendingUsers = await _userManager.Users.Where(u => u.Status == UserStatus.PendingApproval).ToListAsync();
            var result = pendingUsers.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                Roles = _userManager.GetRolesAsync(u).Result,
                u.Status
            });
            return Ok(ApiResponse<object>.Success(result, "Fetched pending users successfully"));
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
                u.Status
            });
            return Ok(ApiResponse<object>.Success(result, "Fetched all adopters successfully"));
        }

        [HttpGet("all-owners")]
        public async Task<IActionResult> GetAllOwners()
        {
            var allOwners = await _userManager.GetUsersInRoleAsync("Owner");
            var result = allOwners.Select(u => new
            {
                u.Id,
                u.FullName,
                u.Email,
                u.Status
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
            user.Status = UserStatus.Approved;

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
            if (user.Status == UserStatus.Approved)
                return BadRequest(ApiResponse<object>.Fail("User is already approved, cannot reject"));

            // Reject the user
            user.Status = UserStatus.Rejected;
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


        //Dashboard stats
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await _dashboardService.GetDashboardStatsAsync();
            return Ok(ApiResponse<DashboardDto>.Success(result));
        }
    }
}
