using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<List<Invoice>> GetOverdueInvoicesAsync(Guid tenantId);
    }
}
