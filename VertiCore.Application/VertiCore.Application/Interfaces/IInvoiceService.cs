using VertiCore.Application.DTOs.Invoice;

namespace VertiCore.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<List<InvoiceDto>> GetAllAsync(Guid tenantId);
        Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request, Guid tenantId);
        Task<List<InvoiceDto>> GetOverdueAsync(Guid tenantId);
    }
}
