using PetAdopt.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IHostEnvironment _env;

        //?? throw new ArgumentNullException(nameof(env))
        public FileService(IHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadFileAsync(Stream stream, string fileName)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, "wwwroot/Images", fileName);

            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await stream.CopyToAsync(fileStream);
            }

            return "/Images/" + fileName;
        }
    }
}
