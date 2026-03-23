using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webappi.Data
{
    public class ErpSemester : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // e.g., Semester 1

        [Required]
        public string Code { get; set; } = string.Empty; // e.g., SEM1

        public int SequenceNo { get; set; } // 1, 2, 3...

        public int ProgramId { get; set; }
        [ForeignKey("ProgramId")]
        public virtual ErpProgram? Program { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
