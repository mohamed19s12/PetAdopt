using PetAdopt.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Entities
{
    public class Review : BaseEntity
    {
        public string ReviewerId { get; set; }
        public ApplicationUser Reviewer { get; set; }

        public string TargetUserId { get; set; }
        public ApplicationUser TargetUser { get; set; }

        public int PetId { get; set; }
        public Pet Pet { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
    }
}
