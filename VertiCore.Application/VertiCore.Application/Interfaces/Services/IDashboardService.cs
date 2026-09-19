using VertiCore.Application.DTOs.Dashboard;

namespace VertiCore.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(Guid tenantId);
    }
}

