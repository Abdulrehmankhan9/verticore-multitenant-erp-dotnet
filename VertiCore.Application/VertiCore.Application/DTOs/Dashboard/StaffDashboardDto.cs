namespace VertiCore.Application.DTOs.Dashboard
{
    public class StaffDashboardDto
    {
        public int TotalClientsCount { get; set; }
        public int ActiveClientsCount { get; set; }
        public int MyActionsThisWeek { get; set; }
        public int AssignedTasksCount { get; set; }
        public int OpenTasksCount { get; set; }
        public List<StaffActivityDto> RecentActivity { get; set; } = new();
    }

    public class StaffActivityDto
    {
        public string Action { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}