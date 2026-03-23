using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webappi.Data
{
    public class ErpBranch : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., Computer Science

        [Required]
        public string Code { get; set; } = string.Empty; // e.g., CSE

        public int ProgramId { get; set; }
        [ForeignKey("ProgramId")]
        public virtual ErpProgram? Program { get; set; }

        // Link to Administrative Department (Optional but recommended for Staff filtering)
        public int? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual ErpDepartment? Department { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
