using VertiCore.Domain.Common;

namespace VertiCore.Domain.Entities
{
    public class Client : BaseEntity
    {
        public Guid TenantId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public Tenant Tenant { get; set; } = null!;
    }
}
