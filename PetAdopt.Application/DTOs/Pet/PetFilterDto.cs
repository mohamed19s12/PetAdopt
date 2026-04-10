using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DTOs.Pet
{
    public class PetFilterDto
    {
        //All are nullable because User can choose to filter by any of these
        public string? SearchTerm { get; set; }
        public int? Age { get; set; }

        //For Sorting
        public SortBy SortBy { get; set; } = SortBy.Name; //name , age
        public bool IsDescending { get; set; } = false;

        //For Pagination
        public int Page { get; set; } = 1; // Default to first page
        public int PageSize { get; set; } = 10; // Default page size
    }
}
