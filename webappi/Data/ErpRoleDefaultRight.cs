using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webappi.Data; 

namespace webappi.Data
{
    public class ErpRoleDefaultRight : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual webappi.Data.AppRole Role { get; set; }

        public int PageId { get; set; }
        [ForeignKey("PageId")]
        public virtual ErpPage Page { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
