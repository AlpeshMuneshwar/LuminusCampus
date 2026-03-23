using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webappi.Data
{
    public class ErpSubscription : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }

        [Required]
        public int ModuleId { get; set; }
        [ForeignKey(nameof(ModuleId))]
        public ErpModule? Module { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
