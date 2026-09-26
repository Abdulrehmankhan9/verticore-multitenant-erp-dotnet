using VertiCore.Application.DTOs.Task;
using VertiCore.Application.Exceptions;
using VertiCore.Application.Interfaces.Repositories;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Enums;
using VertiCore.Domain.Exceptions;

namespace VertiCore.Application.Services
{
    public class TaskService : ITaskService
    {
        private readonly IRepository<WorkTask> _taskRepository;
        private readonly IUserRepository _userRepository;

        public TaskService(IRepository<WorkTask> taskRepository, IUserRepository userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
        }

        public async Task<List<TaskDto>> GetTasksAsync(Guid tenantId, Guid? assignedUserId = null)
        {
            var tasks = (await _taskRepository.GetAllAsync())
                .Where(task => task.TenantId == tenantId &&
                    (assignedUserId == null || task.AssignedUserId == assignedUserId))
                .OrderBy(task => task.DueDate)
                .ThenByDescending(task => task.CreatedAt)
                .ToList();

            var result = new List<TaskDto>();
            foreach (var task in tasks)
            {
                var assignedUser = await _userRepository.GetByIdAndTenantAsync(task.AssignedUserId, tenantId);
                result.Add(new TaskDto
                {
                    Id = task.Id,
                    AssignedUserId = task.AssignedUserId,
                    AssignedUserName = assignedUser?.FullName ?? "Unknown",
                    Title = task.Title,
                    Description = task.Description,
                    DueDate = task.DueDate,
                    Status = task.Status,
                    CreatedAt = task.CreatedAt
                });
            }

            return result;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskRequest request, Guid tenantId)
        {
            var assignee = await _userRepository.GetByIdAndTenantAsync(request.AssignedUserId, tenantId);
            if (assignee == null || !assignee.IsActive || assignee.Role != UserRole.Staff)
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    [nameof(request.AssignedUserId)] = ["Choose an active staff member in this workspace"]
                });

            var task = new WorkTask
            {
                TenantId = tenantId,
                AssignedUserId = assignee.Id,
                Title = request.Title.Trim(),
                Description = request.Description.Trim(),
                DueDate = request.DueDate,
                Status = WorkTaskStatus.Open
            };

            await _taskRepository.AddAsync(task);
            await _taskRepository.SaveChangesAsync();

            return new TaskDto
            {
                Id = task.Id,
                AssignedUserId = task.AssignedUserId,
                AssignedUserName = assignee.FullName,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                Status = task.Status,
                CreatedAt = task.CreatedAt
            };
        }

        public async Task UpdateStatusAsync(Guid taskId, WorkTaskStatus status, Guid tenantId, Guid? assignedUserId = null)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            if (task == null || task.TenantId != tenantId ||
                (assignedUserId.HasValue && task.AssignedUserId != assignedUserId.Value))
                throw new WorkTaskNotFoundException(taskId);

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            _taskRepository.Update(task);
            await _taskRepository.SaveChangesAsync();
        }
    }
}