using PetAdopt.Domain.Common;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Entities
{
    public class AdoptionRequest : BaseEntity
    {
        public int PetId { get; set; }
        public Pet Pet { get; set; }

        public string AdoprerId { get; set; }
        public ApplicationUser Adopter { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Pending;


        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }
}
