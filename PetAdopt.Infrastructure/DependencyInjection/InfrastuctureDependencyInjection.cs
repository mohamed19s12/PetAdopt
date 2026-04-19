using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.Interfaces.Services;
using PetAdopt.Application.Services;
using PetAdopt.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Infrastructure.DependencyInjection
{
    public static class InfrastuctureDependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Services
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITokenService, TokenService>();

            //Email Service
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            services.AddScoped<IEmailService, EmailService>();

            return services;
        }
    }
}
