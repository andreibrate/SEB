using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEB.Models.Enums
{
    public enum Rank
    {
        Loser,     // < 100
        Newbie,    // == 100
        Advanced,  // > 100 && <= 105
        Master     // > 105
    }
}
