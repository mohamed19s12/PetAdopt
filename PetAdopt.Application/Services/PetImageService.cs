using Microsoft.AspNetCore.Http;
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

        public PetImageService(IPetImageRepository repo, IFileService fileService)
        {
            _repo = repo;
            _fileService = fileService;
        }

        public async Task<List<string>> GetPetImages(int petId)
        {
            var images =await _repo.GetByPetIdAsync(petId);
            return images.Select(i => i.ImageUrl).ToList();
        }

        public async Task UploadImage(int petId, Stream stream, string fileName)
        {
            var imageUrl = await _fileService.UploadFileAsync(stream , fileName);

            var petImage = new PetImage
            {
                PetId = petId,
                ImageUrl = imageUrl
            };

            await _repo.AddAsync(petImage);
            await _repo.SaveChangesAsync();
        }
    }
}
