namespace VertiCore.Domain.Events
{
    public class InvoiceCreatedEvent
    {
        public Guid InvoiceId { get; }
        public Guid TenantId { get; }
        public decimal TotalAmount { get; }
        public DateTime CreatedAt { get; }

        public InvoiceCreatedEvent(Guid invoiceId, Guid tenantId, decimal totalAmount)
        {
            InvoiceId = invoiceId;
            TenantId = tenantId;
            TotalAmount = totalAmount;
            CreatedAt = DateTime.UtcNow;
        }
    }
}