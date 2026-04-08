using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface IAdoptionService
    {
        Task Apply(string userId , int petId);
        Task Acceept(int requestId);
        Task Reject(int requestId);
    }
}
