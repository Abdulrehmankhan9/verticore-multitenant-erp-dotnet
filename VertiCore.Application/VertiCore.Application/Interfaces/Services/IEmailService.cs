namespace VertiCore.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string tenantName, string fullName);
        Task SendInvitationEmailAsync(string toEmail, string fullName, string tenantName, string token);
    }
}
