using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(Stream stream, string fileName);
    }
}
