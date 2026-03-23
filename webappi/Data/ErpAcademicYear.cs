using System.ComponentModel.DataAnnotations;

namespace webappi.Data
{
    public class ErpAcademicYear : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., 2025-2026

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
