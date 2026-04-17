using PetAdopt.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Entities
{
    public class PetImage : BaseEntity
    {
        public int PetId { get; set; }
        public Pet Pet { get; set; }    

        public string ImageUrl { get; set; }
    }
}
