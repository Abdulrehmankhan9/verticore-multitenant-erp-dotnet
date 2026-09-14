using VertiCore.Domain.Common;
using VertiCore.Domain.Enums;

namespace VertiCore.Domain.Entities
{
    public class UserInvitation : BaseEntity
    {
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string InvitationToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;

        public Tenant Tenant { get; set; } = null!;
    }
}