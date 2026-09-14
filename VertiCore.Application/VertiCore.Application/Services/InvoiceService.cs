using VertiCore.Application.DTOs.Invoice;
using VertiCore.Application.Interfaces;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Enums;
using VertiCore.Domain.Exceptions;

namespace VertiCore.Application.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IInvoiceItemRepository _invoiceItemRepository;

        public InvoiceService(
            IInvoiceRepository invoiceRepository,
            IClientRepository clientRepository,
            IInvoiceItemRepository invoiceItemRepository)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _invoiceItemRepository = invoiceItemRepository;
        }

        public async Task<List<InvoiceDto>> GetAllAsync(Guid tenantId)
        {
            var tenantInvoices = await _invoiceRepository.GetAllAsync();

            foreach (var invoice in tenantInvoices)
            {
                if (invoice.DueDate < DateTime.UtcNow &&
                    invoice.Status != InvoiceStatus.Paid &&
                    invoice.Status != InvoiceStatus.Overdue)
                {
                    invoice.Status = InvoiceStatus.Overdue;
                    _invoiceRepository.Update(invoice);
                }
            }
            await _invoiceRepository.SaveChangesAsync();

            var result = new List<InvoiceDto>();
            foreach (var invoice in tenantInvoices)
            {
                var client = await _clientRepository.GetByIdAsync(invoice.ClientId);
                result.Add(new InvoiceDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    ClientName = client?.FullName ?? "Unknown",
                    Status = invoice.Status,
                    TotalAmount = invoice.TotalAmount,
                    DueDate = invoice.DueDate,
                    CreatedAt = invoice.CreatedAt
                });
            }

            return result;
        }

        public async Task<InvoiceDto> CreateAsync(CreateInvoiceRequest request, Guid tenantId)
        {
            var totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);
            var invoiceNumber = $"INV-{DateTime.UtcNow.Year}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var invoice = new Invoice
            {
                TenantId = tenantId,
                ClientId = request.ClientId,
                InvoiceNumber = invoiceNumber,
                Status = InvoiceStatus.Draft,
                TotalAmount = totalAmount,
                DueDate = request.DueDate,
                Notes = request.Notes
            };

            await _invoiceRepository.AddAsync(invoice);
            await _invoiceRepository.SaveChangesAsync();

            var invoiceItems = request.Items.Select(i => new InvoiceItem
            {
                InvoiceId = invoice.Id,
                Description = i.Description,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Total = i.Quantity * i.UnitPrice
            }).ToList();

            await _invoiceItemRepository.AddRangeAsync(invoiceItems);
            await _invoiceItemRepository.SaveChangesAsync();

            var client = await _clientRepository.GetByIdAsync(request.ClientId);

            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceNumber = invoice.InvoiceNumber,
                ClientName = client?.FullName ?? "Unknown",
                Status = invoice.Status,
                TotalAmount = invoice.TotalAmount,
                DueDate = invoice.DueDate,
                CreatedAt = invoice.CreatedAt
            };
        }

        public async Task<List<InvoiceDto>> GetOverdueAsync(Guid tenantId)
        {
            var overdueInvoices = await _invoiceRepository.GetOverdueInvoicesAsync(tenantId);

            var result = new List<InvoiceDto>();
            foreach (var invoice in overdueInvoices)
            {
                var client = await _clientRepository.GetByIdAsync(invoice.ClientId);
                result.Add(new InvoiceDto
                {
                    Id = invoice.Id,
                    InvoiceNumber = invoice.InvoiceNumber,
                    ClientName = client?.FullName ?? "Unknown",
                    Status = invoice.Status,
                    TotalAmount = invoice.TotalAmount,
                    DueDate = invoice.DueDate,
                    CreatedAt = invoice.CreatedAt
                });
            }

            return result;
        }

        public async Task UpdateStatusAsync(Guid invoiceId, InvoiceStatus status, Guid tenantId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);

            if (invoice == null || invoice.TenantId != tenantId)
                throw new InvoiceNotFoundException(invoiceId);

            invoice.Status = status;
            _invoiceRepository.Update(invoice);
            await _invoiceRepository.SaveChangesAsync();
        }
    }
}   