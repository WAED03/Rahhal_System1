using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    public class Country : ISoftDeletable
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
        public string Continent { get; set; }

        // خصائص الحذف الناعم
        public bool IsDeleted { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

