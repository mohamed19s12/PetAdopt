using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DTOs.Pet
{
    public class PetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }

        //Age For Sort
        public int Age { get; set; }

        public List<string> Images { get; set; } = new();
    }
}
