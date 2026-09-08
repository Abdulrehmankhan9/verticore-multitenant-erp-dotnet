using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public ClientController(IClientService clientService)
        {
            _clientService = clientService;
        }

        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("TenantId")?.Value;
            return Guid.Parse(tenantIdClaim!);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tenantId = GetTenantId();
            var clients = await _clientService.GetAllAsync(tenantId);
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var client = await _clientService.GetByIdAsync(id, tenantId);

            if (client == null)
                return NotFound();

            return Ok(client);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateClientRequest request)
        {
            var tenantId = GetTenantId();
            var client = await _clientService.CreateAsync(request, tenantId);
            return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateClientRequest request)
        {
            var tenantId = GetTenantId();
            await _clientService.UpdateAsync(id, request, tenantId);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var tenantId = GetTenantId();
            await _clientService.DeleteAsync(id, tenantId);
            return NoContent();
        }
    }
}