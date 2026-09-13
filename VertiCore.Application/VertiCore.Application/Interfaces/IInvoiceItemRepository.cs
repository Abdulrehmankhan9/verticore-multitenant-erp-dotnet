using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces
{
    public interface IInvoiceItemRepository
    {
        Task AddRangeAsync(List<InvoiceItem> items);
        Task SaveChangesAsync();
    }
}