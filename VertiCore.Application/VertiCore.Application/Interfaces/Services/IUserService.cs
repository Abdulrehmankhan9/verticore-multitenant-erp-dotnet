using VertiCore.Application.DTOs.User;

namespace VertiCore.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task InviteUserAsync(InviteUserRequest request, Guid tenantId);
        Task SetPasswordAsync(SetPasswordRequest request);
    }
}
