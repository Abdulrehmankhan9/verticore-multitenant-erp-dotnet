namespace VertiCore.Application.DTOs.Dashboard
{
    public class DashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal OutstandingAmount { get; set; }
        public int OverdueInvoicesCount { get; set; }
        public int ActiveClientsCount { get; set; }
        public List<TopClientDto> TopClients { get; set; } = new();
    }

    public class TopClientDto
    {
        public string ClientName { get; set; } = string.Empty;
        public decimal TotalRevenue { get; set; }
    }
}
