using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Models
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? UpdatedAt { get; set; }
    }
}

