using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    public class MessageModel
    {
        public int user_id { get; set; }
        public string username { get; set; }
        public string message { get; set; }
        public bool is_read { get; set; }
        public string system_id { get; set; }
        public string created_at { get; set; }
    }

}

