using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.DTOs.Invoice;
using VertiCore.Application.Interfaces;

namespace VertiCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
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
            var invoices = await _invoiceService.GetAllAsync(tenantId);
            return Ok(invoices);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInvoiceRequest request)
        {
            var tenantId = GetTenantId();
            var invoice = await _invoiceService.CreateAsync(request, tenantId);
            return Ok(invoice);
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdue()
        {
            var tenantId = GetTenantId();
            var invoices = await _invoiceService.GetOverdueAsync(tenantId);
            return Ok(invoices);
        }
    }
}