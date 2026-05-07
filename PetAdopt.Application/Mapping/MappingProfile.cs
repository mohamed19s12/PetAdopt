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
               .ForMember(dest => dest.PetStatusForAdoption, opt => opt.MapFrom(src => src.petStatusForAdoption.ToString()))
               .ForMember(dest => dest.PostsApprovalStatus, opt => opt.MapFrom(src => src.postsApprovalStatus.ToString()))
               .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                     src.Images != null ? src.Images.Select(i => i.ImageUrl).ToList() : new List<string>()))
               .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src =>
                     src.Owner != null ? src.Owner.FullName : ""))
               .ForMember(dest => dest.OwnerRating, opt => opt.MapFrom(src =>
                     src.Owner != null && src.Owner.ReviewsReceived != null && src.Owner.ReviewsReceived.Any()
                         ? Math.Round(src.Owner.ReviewsReceived.Average(r => (double)r.Rating), 1)
                         : 0.0));

            //creating , updating
            CreateMap<CreatePetDto, Pet>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());

            CreateMap<UpdatePetDto, Pet>()
                .ForMember(dest => dest.Images, opt => opt.Ignore());

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
                .ForMember(dest => dest.RequestStatus,  opt => opt.MapFrom(src => src.RequestStatus.ToString()))
                .ForMember(dest => dest.PetStatusForAdoption,  opt => opt.MapFrom(src => src.PetStatusForAdoption.ToString()));

            // Review Mappings //
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ReviewerName,
                    opt => opt.MapFrom(src => src.Reviewer.FullName));

            CreateMap<CreateReviewDto, Review>();

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
                .ForMember(dest => dest.RequestStatus,
                    opt => opt.MapFrom(src => src.Pet.requestStatus.ToString()));

            //// Favorite //
            //CreateMap<Favorite, PetDto>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Pet.Id))
            //    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Pet.Name))
            //    .ForMember(dest => dest.Breed, opt => opt.MapFrom(src => src.Pet.Breed))
            //    .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Pet.Gender))
            //    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Pet.Description))
            //    .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Pet.Location))
            //    .ForMember(dest => dest.HealthStatus, opt => opt.MapFrom(src => src.Pet.HealthStatus))
            //    .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Pet.Age))
            //    .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Pet.Images.Select(i => i.ImageUrl)))
            //    .ForMember(dest => dest.PetStatusForAdoption, opt => opt.MapFrom(src => src.Pet.petStatusForAdoption.ToString()))
            //    .ForMember(dest => dest.PostsApprovalStatus, opt => opt.MapFrom(src => src.Pet.postsApprovalStatus.ToString()))
            //    .ForMember(dest => dest.RequestStatus, opt => opt.MapFrom(src => src.Pet.requestStatus.ToString()))
            //    .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Pet.Owner.FullName))
            //    .ForMember(dest => dest.OwnerRating, opt => opt.MapFrom(src =>
            //        src.Pet.Owner.ReviewsReceived.Any()
            //            ? src.Pet.Owner.ReviewsReceived.Average(r => r.Rating)
            //            : 0
            //    ));
        }
    }
}
