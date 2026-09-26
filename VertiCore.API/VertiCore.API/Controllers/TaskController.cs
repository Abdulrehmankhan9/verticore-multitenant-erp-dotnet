using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.DTOs;
using VertiCore.Application.DTOs.Task;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Domain.Enums;

namespace VertiCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "StaffAndManagers")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tenantId = Guid.Parse(User.FindFirst("TenantId")!.Value);
            var userId = Guid.Parse(User.FindFirst("UserId")!.Value);
            var isStaff = User.IsInRole(nameof(UserRole.Staff));
            var tasks = await _taskService.GetTasksAsync(tenantId, isStaff ? userId : null);
            return Ok(ApiResponse<List<TaskDto>>.Ok(tasks));
        }

        [HttpPost]
        [Authorize(Policy = "ManagerAndAbove")]
        public async Task<IActionResult> Create(CreateTaskRequest request)
        {
            var tenantId = Guid.Parse(User.FindFirst("TenantId")!.Value);
            var task = await _taskService.CreateTaskAsync(request, tenantId);
            return Ok(ApiResponse<TaskDto>.Ok(task, "Task assigned successfully"));
        }

        [HttpPut("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateTaskStatusRequest request)
        {
            if (!Enum.IsDefined(request.Status))
                return BadRequest(ApiResponse<string>.Fail("Invalid task status"));

            var tenantId = Guid.Parse(User.FindFirst("TenantId")!.Value);
            var isStaff = User.IsInRole(nameof(UserRole.Staff));
            var userId = isStaff ? Guid.Parse(User.FindFirst("UserId")!.Value) : (Guid?)null;
            await _taskService.UpdateStatusAsync(id, request.Status, tenantId, userId);
            return Ok(ApiResponse<string>.Ok("Updated", "Task status updated"));
        }
    }
}