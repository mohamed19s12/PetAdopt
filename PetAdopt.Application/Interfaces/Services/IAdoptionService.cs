using PetAdopt.Application.DTOs.Adoption;
using PetAdopt.Domain.Enums;
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
        Task<List<AdoptionRequestDto>> GetMyRequestsAsync(string adopterId , RequestStatus? status = null);
        Task<List<AdoptionRequestDto>> GetOwnerRequestsAsync(string ownerId);

        Task<List<AdoptionRequestDto>> GetAllRequestsAsync();
    }
}
