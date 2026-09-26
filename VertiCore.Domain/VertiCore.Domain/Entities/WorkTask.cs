using VertiCore.Domain.Common;
using VertiCore.Domain.Enums;

namespace VertiCore.Domain.Entities
{
    public class WorkTask : BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid AssignedUserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public WorkTaskStatus Status { get; set; } = WorkTaskStatus.Open;

        public Tenant Tenant { get; set; } = null!;
        public User AssignedUser { get; set; } = null!;
    }
}