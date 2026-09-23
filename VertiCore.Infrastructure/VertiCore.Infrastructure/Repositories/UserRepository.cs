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
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAndTenantAsync(Guid id, Guid tenantId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
        }
    }
}