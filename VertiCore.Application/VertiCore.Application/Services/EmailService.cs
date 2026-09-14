using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using VertiCore.Application.Interfaces;

namespace VertiCore.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private SmtpClient CreateSmtpClient()
        {
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var appPassword = _configuration["EmailSettings:AppPassword"];

            return new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(senderEmail, appPassword),
                EnableSsl = true
            };
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string tenantName, string fullName)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];

            var client = CreateSmtpClient();

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = "Welcome to VertiCore!",
                Body = $@"
                    <h2>Welcome to VertiCore, {fullName}!</h2>
                    <p>Your business <strong>{tenantName}</strong> has been successfully registered.</p>
                    <p>You can now login and start managing your clients and invoices.</p>
                    <br/>
                    <p>Best regards,</p>
                    <p>VertiCore Team</p>
                ",
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
        }

        public async Task SendInvitationEmailAsync(string toEmail, string fullName, string tenantName, string token)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];

            var client = CreateSmtpClient();

            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var setPasswordLink = $"{baseUrl}/Auth/SetPassword?token={token}";

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = $"You've been invited to join {tenantName} on VertiCore",
                Body = $@"
                    <h2>Hello {fullName}!</h2>
                    <p><strong>{tenantName}</strong> has invited you to join VertiCore.</p>
                    <p>Click the button below to set your password and get started:</p>
                    <br/>
                    <a href='{setPasswordLink}' 
                       style='background-color: #1a1a1a; color: white; padding: 12px 24px; 
                              text-decoration: none; border-radius: 4px;'>
                       Set Password
                    </a>
                    <br/><br/>
                    <p>This link will expire in 48 hours.</p>
                    <p>Best regards,</p>
                    <p>VertiCore Team</p>
                ",
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
        }
    }
}