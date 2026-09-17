using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces.Repositories
{
    public interface IInvoiceItemRepository
    {
        Task AddRangeAsync(List<InvoiceItem> items);
        Task SaveChangesAsync();
    }
}