using webappi.Models;

namespace webappi.Services.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuSection>> GetMenuForUserAsync(int userId, string userRole);
    }

    public class MenuSection
    {
        public string Title { get; set; } = "";
        public List<MenuItem> Items { get; set; } = new();
    }

    public class MenuItem
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "#";
        public string Icon { get; set; } = "lni lni-grid-alt";
        public bool IsActive { get; set; }
        public List<MenuItem> SubItems { get; set; } = new();
    }
}
