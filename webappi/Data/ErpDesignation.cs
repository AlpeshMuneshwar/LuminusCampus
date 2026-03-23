using System.ComponentModel.DataAnnotations;

namespace webappi.Data
{
    public class ErpDesignation : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // e.g. Principal, Teacher, Student

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
