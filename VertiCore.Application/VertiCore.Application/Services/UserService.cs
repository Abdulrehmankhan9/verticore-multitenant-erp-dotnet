using BCrypt.Net;
using VertiCore.Application.DTOs.User;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Application.Interfaces.Repositories;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Exceptions;

namespace VertiCore.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<UserInvitation> _invitationRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IRepository<Tenant> _tenantRepository;

        public UserService(
            IRepository<UserInvitation> invitationRepository,
            IUserRepository userRepository,
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
            if (await _userRepository.GetByEmailAsync(request.Email.Trim()) != null)
                throw new DuplicateUserEmailException();

            var pendingInvitations = await _invitationRepository.GetAllAsync();
            if (pendingInvitations.Any(invitation =>
                !invitation.IsUsed &&
                invitation.ExpiresAt > DateTime.UtcNow &&
                string.Equals(invitation.Email.Trim(), request.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
                throw new DuplicateUserEmailException();

            var token = Guid.NewGuid().ToString();

            var invitation = new UserInvitation
            {
                TenantId = tenantId,
                Email = request.Email.Trim().ToLowerInvariant(),
                FullName = request.FullName,
                Role = request.Role,
                InvitationToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(48),
                IsUsed = false
            };

            await _invitationRepository.AddAsync(invitation);
            await _invitationRepository.SaveChangesAsync();

            var tenant = await _tenantRepository.GetByIdAsync(tenantId);

            try
            {
                await _emailService.SendInvitationEmailAsync(
                    invitation.Email,
                    request.FullName,
                    tenant?.Name ?? "VertiCore",
                    token
                );
            }
            catch
            {
                invitation.IsUsed = true;
                _invitationRepository.Update(invitation);
                await _invitationRepository.SaveChangesAsync();
                throw;
            }
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

            if (await _userRepository.GetByEmailAsync(invitation.Email) != null)
            {
                invitation.IsUsed = true;
                _invitationRepository.Update(invitation);
                await _invitationRepository.SaveChangesAsync();
                throw new DuplicateUserEmailException();
            }

            var user = new User
            {
                TenantId = invitation.TenantId,
                FullName = invitation.FullName,
                Email = invitation.Email.Trim().ToLowerInvariant(),
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

        public async Task<List<UserDto>> GetUsersAsync(Guid tenantId)
        {
            var users = await _userRepository.GetAllAsync();
            return users
                .Where(u => u.TenantId == tenantId)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = (int)u.Role,
                    IsActive = u.IsActive
                }).ToList();
        }
    }
}