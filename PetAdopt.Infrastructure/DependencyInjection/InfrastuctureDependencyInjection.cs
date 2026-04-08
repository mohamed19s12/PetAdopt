using Microsoft.Extensions.DependencyInjection;
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
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            //Services
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<INotificationService, NotificationService>();


            return services;
        }
    }
}
