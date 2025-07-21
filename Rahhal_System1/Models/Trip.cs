using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    public class Trip
    {
        public int TripID { get; set; }
        public int UserID { get; set; }
        public string TripName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TravelMethod { get; set; }
        public string Notes { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? UpdatedAt { get; set; }

    }
}
