using VertiCore.Application.DTOs.Task;
using VertiCore.Domain.Enums;

namespace VertiCore.Application.Interfaces.Services
{
    public interface ITaskService
    {
        Task<List<TaskDto>> GetTasksAsync(Guid tenantId, Guid? assignedUserId = null);
        Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, Guid tenantId);
        Task UpdateStatusAsync(Guid taskId, WorkTaskStatus status, Guid tenantId, Guid? assignedUserId = null);
    }
}