using VertiCore.Application.DTOs.Invoice;

namespace VertiCore.Application.DTOs.Invoice
{
    public class CreateInvoiceRequest
    {
        public Guid ClientId { get; set; }
        public DateTime DueDate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public List<InvoiceItemDto> Items { get; set; } = new();
    }
}
