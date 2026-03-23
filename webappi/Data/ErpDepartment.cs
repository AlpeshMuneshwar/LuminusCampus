using System.ComponentModel.DataAnnotations;

namespace webappi.Data
{
    public class ErpDepartment : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
