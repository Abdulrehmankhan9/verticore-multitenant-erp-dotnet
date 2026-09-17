using BCrypt.Net;
using VertiCore.Application.DTOs.User;
using VertiCore.Application.Interfaces;
using VertiCore.Application.Interfaces.Repositories;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Exceptions;

namespace VertiCore.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<UserInvitation> _invitationRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IEmailService _emailService;
        private readonly IRepository<Tenant> _tenantRepository;

        public UserService(
            IRepository<UserInvitation> invitationRepository,
            IRepository<User> userRepository,
            IEmailService emailService,
            IRepository<Tenant> tenantRepository)
        {
            _invitationRepository = invitationRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _tenantRepository = tenantRepository;
        }

        public async Task InviteUserAsync(InviteUserRequest request, Guid tenantId)
        {
            var token = Guid.NewGuid().ToString();

            var invitation = new UserInvitation
            {
                TenantId = tenantId,
                Email = request.Email,
                FullName = request.FullName,
                Role = request.Role,
                InvitationToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(48),
                IsUsed = false
            };

            await _invitationRepository.AddAsync(invitation);
            await _invitationRepository.SaveChangesAsync();

            var tenant = await _tenantRepository.GetByIdAsync(tenantId);

            await _emailService.SendInvitationEmailAsync(
                request.Email,
                request.FullName,
                tenant?.Name ?? "VertiCore",
                token
            );
        }

        public async Task SetPasswordAsync(SetPasswordRequest request)
        {
            var invitations = await _invitationRepository.GetAllAsync();
            var invitation = invitations.FirstOrDefault(i =>
                i.InvitationToken == request.Token &&
                !i.IsUsed &&
                i.ExpiresAt > DateTime.UtcNow);

            if (invitation == null)
                throw new Exception("Invalid or expired invitation token");

            var user = new User
            {
                TenantId = invitation.TenantId,
                FullName = invitation.FullName,
                Email = invitation.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = invitation.Role,
                IsActive = true
            };

            await _userRepository.AddAsync(user);

            invitation.IsUsed = true;
            _invitationRepository.Update(invitation);

            await _userRepository.SaveChangesAsync();
            await _invitationRepository.SaveChangesAsync();
        }
    }
}