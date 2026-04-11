using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Repositories
{
    public interface IReviewRepository
    {
        Task AddAsync(Review review);
        Task<List<Review>> GetByTargetUserIdAsync(string targetUserId);
        Task<bool> HasAdoptedPetAsync(string adopterId, int petId);
        Task<bool> HasReviewedPetAsync(string reviewerId, int petId);
        Task SaveChangesAsync();
    }
}
