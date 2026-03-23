using System.ComponentModel.DataAnnotations;

namespace webappi.Data
{
    public class ErpProgram : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., B.Tech, MBA

        [Required]
        public string Code { get; set; } = string.Empty; // e.g., BTECH, MBA

        public int DurationInYears { get; set; } // e.g., 4

        public bool IsActive { get; set; } = true;
    }
}
