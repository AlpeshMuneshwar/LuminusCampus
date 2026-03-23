using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webappi.Data
{
    public class ErpUserPageRight : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser User { get; set; }

        [Required]
        public int PageId { get; set; }
        [ForeignKey("PageId")]
        public virtual ErpPage Page { get; set; }

        public bool IsActive { get; set; } = true;
        
        public string? AssignedBy { get; set; }
    }
}
