using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    public class CityVisit
    {
        public int VisitID { get; set; }
        public int TripID { get; set; }
        public int CityID { get; set; }
        public DateTime VisitDate { get; set; }
        public string Rating { get; set; } // From '1' to '5'
        public string Notes { get; set; }
    }
}
