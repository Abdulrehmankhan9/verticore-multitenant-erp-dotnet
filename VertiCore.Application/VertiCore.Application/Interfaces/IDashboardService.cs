using VertiCore.Application.DTOs.Dashboard;

namespace VertiCore.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(Guid tenantId);
    }
}
