using VertiCore.Domain.Common;
using VertiCore.Domain.Enums;

namespace VertiCore.Domain.Entities
{
    public class Invoice : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid ClientId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public decimal TotalAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string Notes { get; set; } = string.Empty;

        public Tenant Tenant { get; set; } = null!;
        public Client Client { get; set; } = null!;
    }
}
