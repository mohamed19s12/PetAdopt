using Microsoft.AspNetCore.Http;
using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface IPetImageService
    {
        Task UploadImage(int petId, Stream stream, string fileName);
        Task<List<string>> GetPetImages(int petId);
    }
}
