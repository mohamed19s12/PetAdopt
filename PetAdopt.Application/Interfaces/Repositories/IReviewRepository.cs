using PetAdopt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<List<Review>> GetByTargetUserIdAsync(string targetUserId);
        Task<bool> HasAdoptedPetAsync(string adopterId, int petId);
        Task<bool> HasReviewedPetAsync(string reviewerId, int petId);
        Task<Review?> GetByPetIdAsync(int petId);

        Task<List<Review>> GetByReviewerIdAsync(string reviewerId);
        //Task<Review> GetByIdAsync(int id);
        //Task DeleteAsync(Review review);

        //Task<List<Review>> GetAllStatsAsync();

        //Task SaveChangesAsync();
    }
}
