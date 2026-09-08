using VertiCore.Application.DTOs.Dashboard;
using VertiCore.Application.Interfaces;
using VertiCore.Domain.Enums;
using VertiCore.Domain.Interfaces;

namespace VertiCore.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;

        public DashboardService(IInvoiceRepository invoiceRepository, IClientRepository clientRepository)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(Guid tenantId)
        {
            var allInvoices = await _invoiceRepository.GetAllAsync();
            var tenantInvoices = allInvoices.Where(i => i.TenantId == tenantId).ToList();

            var allClients = await _clientRepository.GetAllAsync();
            var tenantClients = allClients.Where(c => c.TenantId == tenantId).ToList();

            var totalRevenue = tenantInvoices
                .Where(i => i.Status == InvoiceStatus.Paid)
                .Sum(i => i.TotalAmount);

            var outstandingAmount = tenantInvoices
                .Where(i => i.Status != InvoiceStatus.Paid)
                .Sum(i => i.TotalAmount);

            var overdueCount = tenantInvoices
                .Count(i => i.DueDate < DateTime.UtcNow && i.Status != InvoiceStatus.Paid);

            var activeClientsCount = tenantClients.Count(c => c.IsActive);

            var topClients = tenantInvoices
                .GroupBy(i => i.ClientId)
                .Select(g => new
                {
                    ClientId = g.Key,
                    Total = g.Sum(i => i.TotalAmount)
                })
                .OrderByDescending(x => x.Total)
                .Take(5)
                .ToList();

            var topClientsDto = new List<TopClientDto>();
            foreach (var tc in topClients)
            {
                var client = tenantClients.FirstOrDefault(c => c.Id == tc.ClientId);
                topClientsDto.Add(new TopClientDto
                {
                    ClientName = client?.FullName ?? "Unknown",
                    TotalRevenue = tc.Total
                });
            }

            return new DashboardDto
            {
                TotalRevenue = totalRevenue,
                OutstandingAmount = outstandingAmount,
                OverdueInvoicesCount = overdueCount,
                ActiveClientsCount = activeClientsCount,
                TopClients = topClientsDto
            };
        }
    }
}