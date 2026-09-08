using VertiCore.Application.DTOs.AuditLog;

namespace VertiCore.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(Guid tenantId, Guid userId, string action, string entityName, string entityId, string details);
        Task<List<AuditLogDto>> GetAllAsync(Guid tenantId);
    }
}