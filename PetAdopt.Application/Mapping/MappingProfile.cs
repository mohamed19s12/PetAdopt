using AutoMapper;
using PetAdopt.Application.DTOs.Adoption;
using PetAdopt.Application.DTOs.Auth;
using PetAdopt.Application.DTOs.Pet;
using PetAdopt.Application.DTOs.Review;
using PetAdopt.Domain.Entities;
using PetAdopt.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CreateMap<Source, Destination>();

            // Pet //
            //getting
            CreateMap<Pet, PetDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            //creating , updating
            CreateMap<CreatePetDto, Pet>();
            CreateMap<UpdatePetDto, Pet>();

            // Auth //

            //registering
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Status , opt => opt.MapFrom(src => UserStatus.PendingApproval));

            // Adoption //
            CreateMap<AdoptionRequest, AdoptionRequestDto>()
                .ForMember(dest => dest.PetName, opt => opt.MapFrom(src => src.Pet.Name))
                    .ForMember(dest => dest.AdopterName, opt => opt.MapFrom(src => src.Adopter.FullName))  
                    .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Pet.Owner.FullName))
                .ForMember(dest => dest.Status,  opt => opt.MapFrom(src => src.Status.ToString()));

            // Review Mappings //
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ReviewerName,
                    opt => opt.MapFrom(src => src.Reviewer.FullName));

            // Favorite Mappings //
            CreateMap<Favorite, PetDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Pet.Id))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Pet.Name))
                .ForMember(dest => dest.Breed,
                    opt => opt.MapFrom(src => src.Pet.Breed))
                .ForMember(dest => dest.Location,
                    opt => opt.MapFrom(src => src.Pet.Location))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Pet.Status.ToString()));

            // Pet Image //
        }
    }
}
