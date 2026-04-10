using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return Ok(pets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var pet = await _petService.GetByIdAsync(id);
                return Ok(pet);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Create([FromForm] CreatePetDto dto)
        {
            try
            {
                //Get userId from token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //create pet and return its id
                var petId = await _petService.CreateAsync(dto, userId);
                return Ok(petId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdatePetDto dto)
        {
            try
            {
                //Get userId from token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _petService.UpdateAsync(id, dto, userId);
                return Ok("Pet updated successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Owner")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                //Get userId from token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _petService.DeleteAsync(id, userId);
                return Ok("Pet deleted successfully");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] PetFilterDto filter)
        {
            var pets = await _petService.SearchAsync(filter);
            return Ok(pets);
        }
    }
}
