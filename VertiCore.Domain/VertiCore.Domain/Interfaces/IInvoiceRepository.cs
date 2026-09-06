using VertiCore.Domain.Entities;

namespace VertiCore.Domain.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<List<Invoice>> GetOverdueInvoicesAsync(Guid tenantId);
    }
}