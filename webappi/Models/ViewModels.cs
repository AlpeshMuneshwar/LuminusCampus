using System.ComponentModel.DataAnnotations;

namespace webappi.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserType { get; set; } = "";
        

        public string Designation { get; set; } = "N/A";
        public int? ParentUserId { get; set; }
        public string ParentName { get; set; } = "N/A";
    }

    public class CreateUserRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        [Required]
        public string FullName { get; set; } = "";
        
        public int? DesignationId { get; set; }
    }

    public class SubscriptionRequest 
    {
        public int ModuleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
