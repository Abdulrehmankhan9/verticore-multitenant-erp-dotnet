using VertiCore.Application.DTOs.AuditLog;
using VertiCore.Application.Interfaces;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Interfaces;

namespace VertiCore.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<AuditLog> _auditLogRepository;
        private readonly IRepository<User> _userRepository;

        public AuditLogService(IRepository<AuditLog> auditLogRepository, IRepository<User> userRepository)
        {
            _auditLogRepository = auditLogRepository;
            _userRepository = userRepository;
        }

        public async Task LogAsync(Guid tenantId, Guid userId, string action, string entityName, string entityId, string details)
        {
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = userId,
                Action = action,
                EntityName = entityName,
                EntityId = entityId,
                Details = details,
                IPAddress = "N/A",
                CreatedAt = DateTime.UtcNow
            };

            await _auditLogRepository.AddAsync(log);
            await _auditLogRepository.SaveChangesAsync();
        }

        public async Task<List<AuditLogDto>> GetAllAsync(Guid tenantId)
        {
            var logs = await _auditLogRepository.GetAllAsync();
            var tenantLogs = logs.Where(l => l.TenantId == tenantId).OrderByDescending(l => l.CreatedAt).ToList();

            var result = new List<AuditLogDto>();
            foreach (var log in tenantLogs)
            {
                var user = await _userRepository.GetByIdAsync(log.UserId);
                result.Add(new AuditLogDto
                {
                    Id = log.Id,
                    UserName = user?.FullName ?? "Unknown",
                    Action = log.Action,
                    EntityName = log.EntityName,
                    Details = log.Details,
                    CreatedAt = log.CreatedAt
                });
            }

            return result;
        }
    }
}