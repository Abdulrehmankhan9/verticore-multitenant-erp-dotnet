using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VertiCore.Application.DTOs;
using VertiCore.Application.DTOs.Invoice;
using VertiCore.Application.Interfaces;
using VertiCore.Domain.Enums;

namespace VertiCore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "ManagerAndAbove")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IPdfService _pdfService;
        private readonly IAuditLogService _auditLogService;

        public InvoiceController(
            IInvoiceService invoiceService,
            IPdfService pdfService,
            IAuditLogService auditLogService)
        {
            _invoiceService = invoiceService;
            _pdfService = pdfService;
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
            var invoices = await _invoiceService.GetAllAsync(tenantId);
            return Ok(ApiResponse<List<InvoiceDto>>.Ok(invoices));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInvoiceRequest request)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            var invoice = await _invoiceService.CreateAsync(request, tenantId);

            await _auditLogService.LogAsync(tenantId, userId, "Create", "Invoice",
                invoice.Id.ToString(), $"Created invoice: {invoice.InvoiceNumber} for {invoice.ClientName}");

            return Ok(ApiResponse<InvoiceDto>.Ok(invoice, "Invoice created successfully"));
        }

        [HttpGet("overdue")]
        public async Task<IActionResult> GetOverdue()
        {
            var tenantId = GetTenantId();
            var invoices = await _invoiceService.GetOverdueAsync(tenantId);
            return Ok(ApiResponse<List<InvoiceDto>>.Ok(invoices));
        }

        [HttpGet("{id}/pdf")]
        public IActionResult DownloadPdf(Guid id)
        {
            var pdfBytes = _pdfService.GenerateInvoicePdf(id);
            return File(pdfBytes, "application/pdf", $"invoice_{id}.pdf");
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] InvoiceStatus status)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();
            await _invoiceService.UpdateStatusAsync(id, status, tenantId);

            await _auditLogService.LogAsync(tenantId, userId, "UpdateStatus", "Invoice",
                id.ToString(), $"Invoice status updated to: {status}");

            return Ok(ApiResponse<string>.Ok("Updated", "Invoice status updated successfully"));
        }
    }
}