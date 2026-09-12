using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.Interfaces;

namespace VertiCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditLogController : ControllerBase
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            var tenantId = Guid.Parse(tenantIdClaim!);

            var logs = await _auditLogService.GetAllAsync(tenantId);
            return Ok(logs);
        }
    }
}
