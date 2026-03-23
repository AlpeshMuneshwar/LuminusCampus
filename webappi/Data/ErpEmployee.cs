using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webappi.Data
{
    public class ErpEmployee : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        // --- A. Basic Employee Information ---
        [Required]
        public string EmployeeCode { get; set; } = string.Empty; // Auto-generated or Manual

        [Required]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}".Trim();

        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        
        public string? PhotoPath { get; set; }

        // --- B. Contact Information ---
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        
        // --- Emergency Contact ---
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactNumber { get; set; }

        // --- C. Employment Details ---
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual ErpDepartment? Department { get; set; }

        public int DesignationId { get; set; }
        [ForeignKey("DesignationId")]
        public virtual ErpDesignation? Designation { get; set; }

        public DateTime DateOfJoining { get; set; } = DateTime.UtcNow;

        public string? EmploymentType { get; set; } // Full-time, Part-time, Contract
        
        public string Status { get; set; } = "Active"; // Active, Inactive, On Leave

        // --- Link to Application User (Login) ---
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser? User { get; set; }
    }
}
