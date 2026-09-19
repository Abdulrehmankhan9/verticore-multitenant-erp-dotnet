using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.DTOs;
using VertiCore.Application.DTOs.Dashboard;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Application.Interfaces.Repositories;

namespace VertiCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManagerAndAbove")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            var tenantId = Guid.Parse(tenantIdClaim!);
            var dashboard = await _dashboardService.GetDashboardDataAsync(tenantId);
            return Ok(ApiResponse<DashboardDto>.Ok(dashboard));
        }
    }
}
