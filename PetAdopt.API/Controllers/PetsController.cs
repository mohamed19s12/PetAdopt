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

        [HttpPost]
        [Authorize(Roles ="Owner")]
        public async Task<IActionResult> Create([FromForm] CreatePetDto dto)
        {
            try
            {
                //Get userId from token
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                //create pet and return its id
                var petId = await _petService.CreateAsync(dto, userId);
                return Ok( petId );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
