using VertiCore.Domain.Enums;

namespace VertiCore.Application.DTOs.Task
{
    public class UpdateTaskStatusRequest
    {
        public WorkTaskStatus Status { get; set; }
    }
}