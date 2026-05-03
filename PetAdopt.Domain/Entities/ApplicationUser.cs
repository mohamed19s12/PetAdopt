using Microsoft.AspNetCore.Identity;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public UserStatus Status { get; set; } = UserStatus.PendingApproval;
    }
}
