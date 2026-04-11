using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Application.Services;
using PetAdopt.Application.Validators;
using PetAdopt.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.DependencyInjection
{
    public static class ApplicationDependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //Services
            services.AddScoped<IAuthService , AuthService>();
            services.AddScoped<IPetService , PetService>();
            services.AddScoped<IFavoriteService, FavoriteService>();
            services.AddScoped<IAdoptionService, AdoptionService>();
            services.AddScoped<IPetImageService , PetImageService>();
            services.AddScoped<IReviewService, ReviewService>();

            //Fluent Validation
            services.AddFluentValidationAutoValidation();
            
            // this Only enough to get all assembly validators without the need to specify each one of them
            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();



            return services;
        }
    }
}
