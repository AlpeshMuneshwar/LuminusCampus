using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webappi.Data
{
    [Table("UserSessions")]
    public class UserSession
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public string Token { get; set; } // Or SessionId
        public string IPAddress { get; set; }
        public string Device { get; set; }
        public string Browser { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public DateTime? LastSeen { get; set; }
        public DateTime? LogoutTime { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
