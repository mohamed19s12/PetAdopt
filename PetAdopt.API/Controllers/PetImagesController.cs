using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.Application.Interfaces.Services;

namespace PetAdopt.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetImagesController : ControllerBase
    {
        private readonly IPetImageService _petImageService;

        public PetImagesController(IPetImageService petImageService)
        {
            _petImageService = petImageService;
        }



        [HttpPost("{petId}")]
        public async Task<IActionResult> UploadImage(int petId, List<IFormFile> files)
        {
            //if (file == null || file.Length == 0)
            //    return BadRequest("No file uploaded.");
            //using var stream = file.OpenReadStream();
            //await _petImageService.UploadImage(petId, stream, file.FileName);
            //return Ok("Uploaded");

            if (files == null || !files.Any())
                return BadRequest("No files uploaded");

            foreach (var file in files)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                using var stream = file.OpenReadStream();

                await _petImageService.UploadImage(petId, stream, fileName);
            }

            return Ok("Uploaded");
        }

        [HttpGet("{petId}")]
        public async Task<IActionResult> GetPetImages(int petId)
        {
            var images = await _petImageService.GetPetImages(petId);
            return Ok(images);
        }
    }
}
