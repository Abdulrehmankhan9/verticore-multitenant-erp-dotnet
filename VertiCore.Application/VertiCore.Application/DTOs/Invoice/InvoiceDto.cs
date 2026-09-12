using VertiCore.Domain.Enums;

namespace VertiCore.Application.DTOs.Invoice
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public InvoiceStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
