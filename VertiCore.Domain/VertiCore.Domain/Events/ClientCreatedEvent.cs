namespace VertiCore.Domain.Events
{
    public class ClientCreatedEvent
    {
        public Guid ClientId { get; }
        public string ClientName { get; }
        public Guid TenantId { get; }
        public DateTime CreatedAt { get; }

        public ClientCreatedEvent(Guid clientId, string clientName, Guid tenantId)
        {
            ClientId = clientId;
            ClientName = clientName;
            TenantId = tenantId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}