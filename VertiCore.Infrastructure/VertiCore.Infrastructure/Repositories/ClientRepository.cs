using Microsoft.EntityFrameworkCore;
using VertiCore.Domain.Entities;
using VertiCore.Application.Interfaces;
using VertiCore.Infrastructure.Data;

namespace VertiCore.Infrastructure.Repositories
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository
    {
        public ClientRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Client>> SearchAsync(string keyword)
        {
            return await _context.Clients
                .Where(c => c.FullName.Contains(keyword))
                .ToListAsync();
        }
    }
}
