using VertiCore.Domain.Entities;

namespace VertiCore.Application.Interfaces.Repositories
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<List<Invoice>> GetOverdueInvoicesAsync(Guid tenantId);
    }
}


