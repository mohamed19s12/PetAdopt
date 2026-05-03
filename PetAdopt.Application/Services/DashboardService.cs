using Microsoft.AspNetCore.Identity;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.Interfaces.Repositories;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IPetRepository _petRepository;
        private readonly IAdoptionRequestRepository _adoptionRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardService(
            IPetRepository petRepository,
            IAdoptionRequestRepository adoptionRepository,
            IReviewRepository reviewRepository,
            UserManager<ApplicationUser> userManager)
        {
            _petRepository = petRepository;
            _adoptionRepository = adoptionRepository;
            _reviewRepository = reviewRepository;
            _userManager = userManager;
        }

        public async Task<DashboardDto> GetDashboardStatsAsync()
        {
            var owners = await _userManager.GetUsersInRoleAsync("Owner");
            var adopters = await _userManager.GetUsersInRoleAsync("Adopter");
            var allPets = await _petRepository.GetAllStatsAsync();
            var allRequests = await _adoptionRepository.GetAllStatsAsync();
            var allReviews = await _reviewRepository.GetAllStatsAsync();

            return new DashboardDto
            {
                // Pets
                TotalPets = allPets.Count,
                PendingPets = allPets.Count(p => p.Status == PetStatus.Pending),
                ApprovedPets = allPets.Count(p => p.Status == PetStatus.Approved),
                RejectedPets = allPets.Count(p => p.Status == PetStatus.Rejected),
                AdoptedPets = allPets.Count(p => p.Status == PetStatus.Adopted),

                // Adoptions
                TotalAdoptionRequests = allRequests.Count,
                PendingAdoptionRequests = allRequests.Count(a => a.Status == RequestStatus.Pending),
                ApprovedAdoptionRequests = allRequests.Count(a => a.Status == RequestStatus.Approved),
                RejectedAdoptionRequests = allRequests.Count(a => a.Status == RequestStatus.Rejected),

                // Users
                TotalUsers = owners.Count + adopters.Count,
                TotalOwners = owners.Count,
                TotalAdopters = adopters.Count,
                PendingApprovalUsers = _userManager.Users.Count(u => u.Status == UserStatus.PendingApproval),

                // Reviews
                TotalReviews = allReviews.Count,
                AverageRating = allReviews.Any()
                    ? Math.Round(allReviews.Average(r => (double)r.Rating), 1)
                    : 0
            };
        }
    }
}
