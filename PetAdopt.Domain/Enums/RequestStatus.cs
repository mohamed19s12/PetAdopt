using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Enums
{
    public enum RequestStatus
    {
        Pending, // The adoption request is pending and awaiting review
        Approved, // The adoption request has been approved and the pet is ready for adoption
        Rejected, // The adoption request has been rejected and the pet is not available for adoption
        NoRequest // No adoption request has been made for the pet
    }
}
