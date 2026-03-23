
namespace webappi.Models
{
    public class ManageRightsViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<ModuleRightViewModel> Modules { get; set; } = new List<ModuleRightViewModel>();
    }

    public class ModuleRightViewModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public bool IsSelected { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }
}
