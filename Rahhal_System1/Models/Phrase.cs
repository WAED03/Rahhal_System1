using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    public class Phrase
    {
        public int PhraseID { get; set; }
        public int VisitID { get; set; }
        public string OriginalText { get; set; }
        public string Translation { get; set; }
        public string Language { get; set; }
        public string Notes { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public CityVisit Visit { get; set; }

    }
}
