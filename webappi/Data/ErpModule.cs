using System.ComponentModel.DataAnnotations;

namespace webappi.Data
{
    public class ErpModule : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string ModuleName { get; set; } = string.Empty;
        [Required]
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual ICollection<ErpPage> ErpPages { get; set; } = new List<ErpPage>();
    }
}
