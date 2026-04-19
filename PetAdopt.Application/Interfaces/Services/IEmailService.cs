using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
        Task SendConfirmationEmailAsync(string toEmail, string confirmationLink);
        Task SendResetPasswordEmailAsync(string toEmail, string resetLink);
    }
}
