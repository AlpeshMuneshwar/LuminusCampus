using Microsoft.AspNetCore.Identity;

namespace webappi.Data
{
    public class AppUser : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public string? UserType { get; set; } 
        
        // Audit Fields
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public int? DesignationId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("DesignationId")]
        public virtual ErpDesignation? Designation { get; set; }

        public int? ParentUserId { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("ParentUserId")]
        public virtual AppUser? ParentUser { get; set; }
    }


}
