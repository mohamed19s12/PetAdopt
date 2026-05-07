using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Domain.Enums
{
    public enum PetStatusForAdoption
    {
        Available, //Available for adoption, not yet requested
        Requested, //Adoption request has been made, pending approval
        Adopted //Pet has been adopted
    }
}
