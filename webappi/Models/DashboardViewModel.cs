namespace webappi.Models
{
    public class DashboardViewModel
    {
        public int TotalClients { get; set; }
        public int TotalModules { get; set; }
        public int TotalPages { get; set; }
        // Simple dictionary for chart/list: "SIS" -> 5 pages
        public Dictionary<string, int> PagesPerModule { get; set; } = new();
    }
}
