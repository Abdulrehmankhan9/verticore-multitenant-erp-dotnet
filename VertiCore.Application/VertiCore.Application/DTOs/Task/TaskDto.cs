using VertiCore.Domain.Enums;

namespace VertiCore.Application.DTOs.Task
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public Guid AssignedUserId { get; set; }
        public string AssignedUserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
        public WorkTaskStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}