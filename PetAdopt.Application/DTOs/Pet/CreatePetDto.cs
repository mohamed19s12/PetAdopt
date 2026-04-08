using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DTOs.Pet
{
    public class CreatePetDto
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Breed { get; set; }
        public string Gender { get; set; }
        public string HealthStatus { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

    }
}
