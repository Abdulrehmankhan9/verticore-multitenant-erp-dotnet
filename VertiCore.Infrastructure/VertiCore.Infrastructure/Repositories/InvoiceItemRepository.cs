using VertiCore.Domain.Entities;
using VertiCore.Application.Interfaces;
using VertiCore.Infrastructure.Data;

namespace VertiCore.Infrastructure.Repositories
{
    public class InvoiceItemRepository : IInvoiceItemRepository
    {
        private readonly AppDbContext _context;

        public InvoiceItemRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(List<InvoiceItem> items)
        {
            await _context.InvoiceItems.AddRangeAsync(items);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}