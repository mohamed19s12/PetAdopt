using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DTOs
{
    public class DashboardDto
    {
        // Pets Stats
        public int TotalPets { get; set; }
        public int PendingPets { get; set; }
        public int ApprovedPets { get; set; }
        public int RejectedPets { get; set; }
        public int AdoptedPets { get; set; }

        // Adoption Stats
        public int TotalAdoptionRequests { get; set; }
        public int PendingAdoptionRequests { get; set; }
        public int ApprovedAdoptionRequests { get; set; }
        public int RejectedAdoptionRequests { get; set; }

        // Users Stats
        public int TotalUsers { get; set; }
        public int TotalOwners { get; set; }
        public int TotalAdopters { get; set; }
        public int PendingApprovalUsers { get; set; }

        // Reviews Stats
        public int TotalReviews { get; set; }
        public double AverageRating { get; set; }
    }
}
