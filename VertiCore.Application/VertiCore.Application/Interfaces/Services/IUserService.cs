using VertiCore.Application.DTOs.User;

namespace VertiCore.Application.Interfaces
{
    public interface IUserService
    {
        Task InviteUserAsync(InviteUserRequest request, Guid tenantId);
        Task SetPasswordAsync(SetPasswordRequest request);
    }
}