using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DTOs.Adoption
{
    public class AdoptionRequestDto
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public string PetName { get; set; }
        public string AdopterName { get; set; }
        public string OwnerName { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}
