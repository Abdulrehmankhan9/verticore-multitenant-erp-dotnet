using VertiCore.Domain.Common;

namespace VertiCore.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid UserId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;

        public Tenant Tenant { get; set; } = null!;
        public User PerformedBy { get; set; } = null!;
    }
}