using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    public class User : ISoftDeletable
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime JoinDate { get; set; }
        public string Role { get; set; } // Admin or Regular
        public int FailedAttempts { get; set; }
        public DateTime? LastAttempt { get; set; }

        // من الواجهة ISoftDeletable
        public bool IsDeleted { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

