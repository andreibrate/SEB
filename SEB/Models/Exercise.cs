using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEB.Models
{
    public class Exercise
    {
        public int Count { get; set; }
        public int Duration { get; set; } // seconds
        public DateTime Timestamp { get; set; }

        public Exercise()
        {
            Count = 0;
            Duration = 0;
            Timestamp = DateTime.UtcNow;
        }
    }
}
