using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using VertiCore.Application.Interfaces.Services;

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
            var baseUrl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/');
            var loginLink = $"{baseUrl}/login";

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = "Welcome to VertiCore!",
                Body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                </head>
                <body style='margin:0; padding:0; background-color:#f4f4f4; font-family: Arial, sans-serif;'>
                    
                    <table width='100%' cellpadding='0' cellspacing='0'>
                        <!-- Header -->
                        <tr>
                            <td align='center' style='padding: 40px 0 20px 0;'>
                                <table width='600' cellpadding='0' cellspacing='0' 
                                       style='background-color:#1a1a1a; border-radius:8px 8px 0 0;'>
                                    <tr>
                                        <td align='center' style='padding: 30px;'>
                                            <h1 style='color:#ffffff; margin:0; font-size:28px; 
                                                       letter-spacing:2px;'>VertiCore</h1>
                                            <p style='color:#aaaaaa; margin:8px 0 0 0; font-size:13px;'>
                                                Multi-Tenant Business Management
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                
                        <!-- Body -->
                        <tr>
                            <td align='center'>
                                <table width='600' cellpadding='0' cellspacing='0'
                                       style='background-color:#ffffff;'>
                                    <tr>
                                        <td style='padding: 40px 40px 20px 40px;'>
                                            <h2 style='color:#1a1a1a; margin:0 0 16px 0;'>
                                                Welcome, {fullName}! 👋
                                            </h2>
                                            <p style='color:#555555; font-size:15px; line-height:1.6; margin:0 0 20px 0;'>
                                                Your business has been successfully registered on VertiCore.
                                            </p>
                
                                            <!-- Business Name Card -->
                                            <table width='100%' cellpadding='0' cellspacing='0'
                                                   style='background-color:#f8f8f8; border-left:4px solid #1a1a1a; 
                                                          border-radius:4px; margin-bottom:24px;'>
                                                <tr>
                                                    <td style='padding:16px 20px;'>
                                                        <p style='margin:0; color:#888888; font-size:12px;'>
                                                            BUSINESS NAME
                                                        </p>
                                                        <p style='margin:4px 0 0 0; color:#1a1a1a; 
                                                                  font-size:18px; font-weight:bold;'>
                                                            {tenantName}
                                                        </p>
                                                    </td>
                                                </tr>
                                            </table>
                
                                            <p style='color:#555555; font-size:15px; line-height:1.6; margin:0 0 30px 0;'>
                                                You can now login and start managing your clients, 
                                                invoices, and team members.
                                            </p>
                
                                            <!-- CTA Button -->
                                            <table cellpadding='0' cellspacing='0'>
                                                <tr>
                                                    <td align='center' 
                                                        style='background-color:#1a1a1a; border-radius:6px;'>
                                                        <a href='{loginLink}'
                                                           style='display:inline-block; padding:14px 32px; 
                                                                  color:#ffffff; text-decoration:none; 
                                                                  font-size:15px; font-weight:bold;'>
                                                            Login to VertiCore →
                                                        </a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                
                                    <!-- Divider -->
                                    <tr>
                                        <td style='padding: 30px 40px 0 40px;'>
                                            <hr style='border:none; border-top:1px solid #eeeeee; margin:0;'>
                                        </td>
                                    </tr>
                
                                    <!-- Features -->
                                    <tr>
                                        <td style='padding: 24px 40px;'>
                                            <p style='color:#888888; font-size:13px; margin:0 0 16px 0;'>
                                                WHAT YOU CAN DO
                                            </p>
                                            <table width='100%'>
                                                <tr>
                                                    <td width='33%' style='padding:0 8px 0 0;'>
                                                        <p style='margin:0; font-size:20px;'>👥</p>
                                                        <p style='margin:4px 0 0 0; color:#1a1a1a; 
                                                                  font-size:13px; font-weight:bold;'>
                                                            Manage Clients
                                                        </p>
                                                    </td>
                                                    <td width='33%' style='padding:0 8px;'>
                                                        <p style='margin:0; font-size:20px;'>🧾</p>
                                                        <p style='margin:4px 0 0 0; color:#1a1a1a; 
                                                                  font-size:13px; font-weight:bold;'>
                                                            Create Invoices
                                                        </p>
                                                    </td>
                                                    <td width='33%' style='padding:0 0 0 8px;'>
                                                        <p style='margin:0; font-size:20px;'>📊</p>
                                                        <p style='margin:4px 0 0 0; color:#1a1a1a; 
                                                                  font-size:13px; font-weight:bold;'>
                                                            View Dashboard
                                                        </p>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                
                        <!-- Footer -->
                        <tr>
                            <td align='center'>
                                <table width='600' cellpadding='0' cellspacing='0'
                                       style='background-color:#1a1a1a; border-radius:0 0 8px 8px;'>
                                    <tr>
                                        <td align='center' style='padding:20px;'>
                                            <p style='color:#aaaaaa; font-size:12px; margin:0;'>
                                                © 2026 VertiCore. All rights reserved.
                                            </p>
                                            <p style='color:#666666; font-size:11px; margin:8px 0 0 0;'>
                                                This email was sent because you registered on VertiCore.
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                
                    </table>
                </body>
                </html>",
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
            var setPasswordLink = $"{baseUrl}/set-password?token={Uri.EscapeDataString(token)}";

            var message = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = $"You've been invited to join {tenantName} on VertiCore",
                Body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset='utf-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                </head>
                <body style='margin:0; padding:0; background-color:#f4f4f4; font-family: Arial, sans-serif;'>
                    
                    <table width='100%' cellpadding='0' cellspacing='0'>
                        <!-- Header -->
                        <tr>
                            <td align='center' style='padding: 40px 0 20px 0;'>
                                <table width='600' cellpadding='0' cellspacing='0' 
                                       style='background-color:#1a1a1a; border-radius:8px 8px 0 0;'>
                                    <tr>
                                        <td align='center' style='padding: 30px;'>
                                            <h1 style='color:#ffffff; margin:0; font-size:28px; 
                                                       letter-spacing:2px;'>VertiCore</h1>
                                            <p style='color:#aaaaaa; margin:8px 0 0 0; font-size:13px;'>
                                                Multi-Tenant Business Management
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                
                        <!-- Body -->
                        <tr>
                            <td align='center'>
                                <table width='600' cellpadding='0' cellspacing='0'
                                       style='background-color:#ffffff;'>
                                    <tr>
                                        <td style='padding: 40px 40px 20px 40px;'>
                                            <h2 style='color:#1a1a1a; margin:0 0 16px 0;'>
                                                You've been invited! 🎉
                                            </h2>
                                            <p style='color:#555555; font-size:15px; line-height:1.6; margin:0 0 20px 0;'>
                                                Hello {fullName}, you have been invited to join 
                                                <strong>{tenantName}</strong> on VertiCore.
                                            </p>
                
                                            <!-- Role Card -->
                                            <table width='100%' cellpadding='0' cellspacing='0'
                                                   style='background-color:#f8f8f8; border-left:4px solid #1a1a1a; 
                                                          border-radius:4px; margin-bottom:24px;'>
                                                <tr>
                                                    <td style='padding:16px 20px;'>
                                                        <p style='margin:0; color:#888888; font-size:12px;'>
                                                            INVITED BY
                                                        </p>
                                                        <p style='margin:4px 0 0 0; color:#1a1a1a; 
                                                                  font-size:18px; font-weight:bold;'>
                                                            {tenantName}
                                                        </p>
                                                    </td>
                                                </tr>
                                            </table>
                
                                            <p style='color:#555555; font-size:15px; line-height:1.6; margin:0 0 12px 0;'>
                                                Click the button below to set your password and get started.
                                            </p>
                                            <p style='color:#888888; font-size:13px; margin:0 0 30px 0;'>
                                                ⚠️ This invitation link will expire in <strong>48 hours</strong>.
                                            </p>
                
                                            <!-- CTA Button -->
                                            <table cellpadding='0' cellspacing='0'>
                                                <tr>
                                                    <td align='center' 
                                                        style='background-color:#1a1a1a; border-radius:6px;'>
                                                        <a href='{setPasswordLink}'
                                                           style='display:inline-block; padding:14px 32px; 
                                                                  color:#ffffff; text-decoration:none; 
                                                                  font-size:15px; font-weight:bold;'>
                                                            Set Password & Join →
                                                        </a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                
                                    <!-- Divider -->
                                    <tr>
                                        <td style='padding: 30px 40px 0 40px;'>
                                            <hr style='border:none; border-top:1px solid #eeeeee; margin:0;'>
                                        </td>
                                    </tr>
                
                                    <!-- Note -->
                                    <tr>
                                        <td style='padding: 20px 40px 30px 40px;'>
                                            <p style='color:#888888; font-size:12px; margin:0; line-height:1.6;'>
                                                If you did not expect this invitation, you can safely ignore this email.
                                                This invitation was sent by <strong>{tenantName}</strong>.
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                
                        <!-- Footer -->
                        <tr>
                            <td align='center'>
                                <table width='600' cellpadding='0' cellspacing='0'
                                       style='background-color:#1a1a1a; border-radius:0 0 8px 8px;'>
                                    <tr>
                                        <td align='center' style='padding:20px;'>
                                            <p style='color:#aaaaaa; font-size:12px; margin:0;'>
                                                © 2026 VertiCore. All rights reserved.
                                            </p>
                                            <p style='color:#666666; font-size:11px; margin:8px 0 0 0;'>
                                                This email was sent because someone invited you to VertiCore.
                                            </p>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                
                    </table>
                </body>
                </html>",
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string token)
        {
            var senderEmail = _configuration["EmailSettings:SenderEmail"];
            var senderName = _configuration["EmailSettings:SenderName"];
            var baseUrl = _configuration["AppSettings:BaseUrl"]?.TrimEnd('/');
            var resetLink = $"{baseUrl}/reset-password?token={Uri.EscapeDataString(token)}";

            using var client = CreateSmtpClient();
            using var message = new MailMessage
            {
                From = new MailAddress(senderEmail!, senderName),
                Subject = "Reset your VertiCore password",
                Body = $@"
                    <html>
                    <body style='font-family:Arial,sans-serif;color:#172033'>
                        <h2>Hello {WebUtility.HtmlEncode(fullName)},</h2>
                        <p>We received a request to reset your VertiCore password.</p>
                        <p><a href='{resetLink}'>Reset your password</a></p>
                        <p>This link expires in 30 minutes. If you did not request this, you can ignore this email.</p>
                    </body>
                    </html>",
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
            await client.SendMailAsync(message);
        }
    }
}