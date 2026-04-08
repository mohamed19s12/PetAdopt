using PetAdopt.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Entities
{
    public class Favorite : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int PetId { get; set; }
        public Pet Pet { get; set; }
    }
}
