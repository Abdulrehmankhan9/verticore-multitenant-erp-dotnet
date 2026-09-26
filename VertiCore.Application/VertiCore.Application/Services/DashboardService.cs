using VertiCore.Application.DTOs.Dashboard;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Application.Interfaces.Repositories;
using VertiCore.Domain.Entities;
using VertiCore.Domain.Enums;

namespace VertiCore.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IClientRepository _clientRepository;
        private readonly IRepository<AuditLog> _auditLogRepository;

        public DashboardService(
            IInvoiceRepository invoiceRepository,
            IClientRepository clientRepository,
            IRepository<AuditLog> auditLogRepository)
        {
            _invoiceRepository = invoiceRepository;
            _clientRepository = clientRepository;
            _auditLogRepository = auditLogRepository;
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

        public async Task<StaffDashboardDto> GetStaffDashboardDataAsync(Guid tenantId, Guid userId)
        {
            var tenantClients = (await _clientRepository.GetAllAsync())
                .Where(client => client.TenantId == tenantId)
                .ToList();

            var recentActivity = (await _auditLogRepository.GetAllAsync())
                .Where(log => log.TenantId == tenantId && log.UserId == userId)
                .OrderByDescending(log => log.CreatedAt)
                .ToList();

            var weekAgo = DateTime.UtcNow.AddDays(-7);

            return new StaffDashboardDto
            {
                TotalClientsCount = tenantClients.Count,
                ActiveClientsCount = tenantClients.Count(client => client.IsActive),
                MyActionsThisWeek = recentActivity.Count(log => log.CreatedAt >= weekAgo),
                RecentActivity = recentActivity
                    .Take(5)
                    .Select(log => new StaffActivityDto
                    {
                        Action = log.Action,
                        EntityName = log.EntityName,
                        Details = log.Details,
                        CreatedAt = log.CreatedAt
                    })
                    .ToList()
            };
        }
    }
}
