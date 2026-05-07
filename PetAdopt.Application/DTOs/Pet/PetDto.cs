using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DTOs.Pet
{
    public class PetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string HealthStatus { get; set; }

        //Age For Sort
        public int Age { get; set; }

        public List<string> Images { get; set; }

        public string PetStatusForAdoption { get; set; }
        public string PostsApprovalStatus { get; set; }
        public string RequestStatus { get; set; }

        public string OwnerName { get; set; }
        public int OwnerRating { get; set; }
    }
}
