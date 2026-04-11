using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DTOs.Review
{
    public class CreateReviewDto
    {
        public string TargetUserId { get; set; }
        public int PetId { get; set; }
        public int Rating { get; set; }  // 1 - 5
        public string Comment { get; set; }
    }
}
