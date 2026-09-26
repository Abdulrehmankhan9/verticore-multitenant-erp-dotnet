using Microsoft.EntityFrameworkCore;
using VertiCore.Application.Interfaces.Repositories;
using VertiCore.Domain.Entities;
using VertiCore.Infrastructure.Data;

namespace VertiCore.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            return await _context.Users
                .FirstOrDefaultAsync(user => user.Email == normalizedEmail);
        }

        public async Task<User?> GetByPasswordResetTokenHashAsync(string tokenHash)
        {
            return await _context.Users
                .FirstOrDefaultAsync(user => user.PasswordResetTokenHash == tokenHash);
        }

        public async Task<User?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
        }
    }
}