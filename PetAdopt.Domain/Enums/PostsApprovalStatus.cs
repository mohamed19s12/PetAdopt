using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Enums
{
    public enum PostsApprovalStatus
    {
        Pending, // Approval post is pending
        Approved, // Approval post has been granted
        Rejected // Approval post has been denied
    }
}
