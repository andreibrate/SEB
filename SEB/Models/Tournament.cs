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
        public List<Guid> ParticipantIds { get; set; } = new List<Guid>();
        public List<Guid> WinnerIds { get; set; } = new List<Guid>();
        public bool IsActive { get; set; } = true;
        public bool IsDraw { get; set; }

        // default ctor
        public Tournament()
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            IsActive = true;
        }

        // ctor for already known users
        public Tournament(List<Guid> participantIds)
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            ParticipantIds = participantIds;
            IsActive = true;
        }

        // ctor for loading from DB
        public Tournament(Guid id, DateTime startTime, List<Guid> participantIds, List<Guid> winnerIds, bool isActive, bool isDraw)
        {
            Id = id;
            StartTime = startTime;
            ParticipantIds = participantIds;
            WinnerIds = winnerIds;
            IsActive = isActive;
            IsDraw = isDraw;
        }
    }
}
