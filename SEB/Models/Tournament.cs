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
        public Guid? WinnerId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDraw { get; set; }

        // default ctor
        public Tournament()
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            Users = new List<User>();
            IsActive = true;
        }

        // ctor for already known users
        public Tournament(List<User> users)
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            Users = users;
            IsActive = true;
        }

        // ctor for loading from DB
        public Tournament(Guid id, DateTime startTime, List<User> users, Guid? winnerId, bool isActive, bool isDraw)
        {
            Id = id;
            StartTime = startTime;
            Users = users;
            WinnerId = winnerId;
            IsActive = isActive;
            IsDraw = isDraw;
        }
    }
}
