using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webappi.Data
{
    public class ErpPage : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ModuleId { get; set; }

        [ForeignKey("ModuleId")]
        public virtual ErpModule? Module { get; set; }

        [Required]
        public string PageName { get; set; } = string.Empty;

        [Required]
        public string PageUrl { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string UrlToken { get; set; } = Guid.NewGuid().ToString("N"); // Default for new instances

        public bool IsActive { get; set; } = true;
    }
}
