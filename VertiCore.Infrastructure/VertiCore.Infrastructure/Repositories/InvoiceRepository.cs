using Microsoft.EntityFrameworkCore;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Interfaces;
using VertiCore.Infrastructure.Data;

namespace VertiCore.Infrastructure.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Invoice>> GetOverdueInvoicesAsync(Guid tenantId)
        {
            return await _context.Invoices
                .Where(i => i.TenantId == tenantId)
                .Where(i => i.DueDate < DateTime.UtcNow)
                .Where(i => i.Status != Domain.Enums.InvoiceStatus.Paid)
                .ToListAsync();
        }
    }
}