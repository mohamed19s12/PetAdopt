using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Services
{
    public class PetImageService : IPetImageService
    {
        private readonly IPetImageRepository _repo;
        private readonly IFileService _fileService;
        private readonly ILogger<PetImageService> _logger;

        public PetImageService(IPetImageRepository repo, IFileService fileService, ILogger<PetImageService> logger)
        {
            _repo = repo;
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<List<string>> GetPetImages(int petId)
        {
            _logger.LogInformation("Retrieving images for pet: {PetId}", petId);
            var images =await _repo.GetByPetIdAsync(petId);
            return images.Select(i => i.ImageUrl).ToList();
        }

        public async Task UploadImage(int petId, Stream stream, string fileName)
        {
            _logger.LogInformation("Uploading image for pet: {PetId} with file name: {FileName}", petId, fileName);
            var imageUrl = await _fileService.UploadFileAsync(stream , fileName);

            var petImage = new PetImage
            {
                PetId = petId,
                ImageUrl = imageUrl
            };

            await _repo.AddAsync(petImage);
            await _repo.SaveChangesAsync();
            _logger.LogInformation("Image uploaded for pet: {PetId} with URL: {ImageUrl}", petId, imageUrl);
        }
    }
}
