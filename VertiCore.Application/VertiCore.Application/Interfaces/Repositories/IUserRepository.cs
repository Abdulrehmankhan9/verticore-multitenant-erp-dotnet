using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPasswordResetTokenHashAsync(string tokenHash);
        Task<User?> GetByIdAndTenantAsync(Guid id, Guid tenantId);
    }
}