using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PetAdopt.Application.DTOs;
using PetAdopt.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetAdopt.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {Email}", toEmail);
        }

        public async Task SendConfirmationEmailAsync(string toEmail, string confirmationLink)
        {
            var body = $@"
                <h2>Welcome to PetAdopt!</h2>
                <p>Please confirm your email by clicking the link below:</p>
                <a href='{confirmationLink}' 
                   style='background-color: #4CAF50; color: white; padding: 10px 20px; 
                          text-decoration: none; border-radius: 5px;'>
                    Confirm Email
                </a>
                <p>Link expires in 24 hours.</p>
            ";
            await SendEmailAsync(toEmail, "Confirm Your Email - PetAdopt", body);
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
        {
            var body = $@"
                <h2>Reset Your Password</h2>
                <p>Click the link below to reset your password:</p>
                <a href='{resetLink}'
                   style='background-color: #2196F3; color: white; padding: 10px 20px;
                          text-decoration: none; border-radius: 5px;'>
                    Reset Password
                </a>
                <p>Link expires in 1 hour.</p>
                <p>If you didn't request this, ignore this email.</p>
            ";
            await SendEmailAsync(toEmail, "Reset Password - PetAdopt", body);
        }
    }
}
