using PetAdopt.Domain.Common;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Entities
{
    public class Pet : BaseEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Breed { get; set; }
        public string Description { get; set; }
        public ICollection<PetImage> Images { get; set; }
        public string HealthStatus { get; set; }
        public string Location { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public PetStatus Status { get; set; } = PetStatus.Pending;

        //Animal type for Searching and Filtering
        public string? AnimalType { get; set; }
    }
}
