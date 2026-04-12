using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.Interfaces.Services;
using System.Security.Claims;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;

        public PetsController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(ApiResponse<List<PetDto>>.Success(pets));
        }

        [HttpGet("details/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var pet = await _petService.GetByIdAsync(id);
            return Ok(ApiResponse<PetDto>.Success(pet));
        }


        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create([FromForm] CreatePetDto dto)
        {
            //Get userId from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //create pet and return its id
            var petId = await _petService.CreateAsync(dto, userId);
            return Ok(ApiResponse<int>.Success(petId, "Pet created successfully", 201));
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdatePetDto dto)
        {
            //Get userId from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _petService.UpdateAsync(id, dto, userId);
            return Ok(ApiResponse<object>.Success(null,"Pet updated successfully"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int id)
        {
            //Get userId from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _petService.DeleteAsync(id, userId);
            return Ok(ApiResponse<object>.Success(null, "Pet deleted successfully"));
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] PetFilterDto filter)
        {
            var pets = await _petService.SearchAsync(filter);
            return Ok(ApiResponse<PageResultDto<PetDto>>.Success(pets));
        }

        [HttpGet("owner/my-pets")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> GetMyPets()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();
            var result = await _petService.GetMyPetsAsync(userId);
            return Ok(ApiResponse<List<PetDto>>.Success(result));
        }
    }
}
