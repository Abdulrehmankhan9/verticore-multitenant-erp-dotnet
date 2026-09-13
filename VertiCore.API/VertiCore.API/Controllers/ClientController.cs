using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.DTOs;
using VertiCore.Application.DTOs.Client;
using VertiCore.Application.Interfaces;

namespace VertiCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClientController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IAuditLogService _auditLogService;

        public ClientController(IClientService clientService, IAuditLogService auditLogService)
        {
            _clientService = clientService;
            _auditLogService = auditLogService;
        }

        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            return Guid.Parse(tenantIdClaim!);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            return Guid.Parse(userIdClaim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = GetTenantId();
            var clients = await _clientService.GetAllAsync(tenantId);
            return Ok(ApiResponse<List<ClientDto>>.Ok(clients));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var client = await _clientService.GetByIdAsync(id, tenantId);
            if (client == null)
                return NotFound(ApiResponse<ClientDto>.Fail("Client not found"));
            return Ok(ApiResponse<ClientDto>.Ok(client));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClientRequest request)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            var client = await _clientService.CreateAsync(request, tenantId);
            await _auditLogService.LogAsync(tenantId, userId, "Create", "Client", client.Id.ToString(), $"Created client: {client.FullName}");
            return Ok(ApiResponse<ClientDto>.Ok(client, "Client created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateClientRequest request)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            await _clientService.UpdateAsync(id, request, tenantId);
            await _auditLogService.LogAsync(tenantId, userId, "Update", "Client", id.ToString(), $"Updated client: {request.FullName}");
            return Ok(ApiResponse<string>.Ok("Updated", "Client updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            await _clientService.DeleteAsync(id, tenantId);
            await _auditLogService.LogAsync(tenantId, userId, "Delete", "Client", id.ToString(), "Deleted client");
            return Ok(ApiResponse<string>.Ok("Deleted", "Client deleted successfully"));
        }
    }
}