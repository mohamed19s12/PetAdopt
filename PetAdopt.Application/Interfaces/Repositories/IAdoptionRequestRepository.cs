using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Repositories
{
    public interface IAdoptionRequestRepository
    {
        // Adding a new adoption request to the database
        Task AddAsync(AdoptionRequest request);

        //Getting all adoption requests for a specific owner by their ID
        Task<List<AdoptionRequest>> GetByOwnerIdAsync(string ownerId);

        //Getting all adoption requests for a specific pet by its ID
        Task<AdoptionRequest> GetByIdAsync(int id);

        Task<List<AdoptionRequest>> GetByAdopterIdAsync(string AdopterId , RequestStatus? status = null);

        //Saving changes to the database
        Task SaveChangesAsync();
    }
}
