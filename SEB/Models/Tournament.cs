using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEB.Models
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public List<User> Users { get; set; }

        // default ctor
        public Tournament()
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            Users = new List<User>();
        }

        // ctor for already known users
        public Tournament(List<User> users)
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            Users = users;
        }
    }
}
