using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.Models.Enums;

namespace SEB.Models
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public DateTime StartTime { get; set; }
        public List<Guid> ParticipantIds { get; set; } = new List<Guid>();
        public List<Guid> WinnerIds { get; set; } = new List<Guid>();
        public TournamentStatus Status { get; set; } = TournamentStatus.NotYetStarted;
        public bool IsDraw { get; set; }

        public Tournament()
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            Status = TournamentStatus.NotYetStarted;
        }

        public Tournament(List<Guid> participantIds)
        {
            Id = Guid.NewGuid();
            StartTime = DateTime.UtcNow;
            ParticipantIds = participantIds;
            Status = TournamentStatus.NotYetStarted;
        }

        public Tournament(Guid id, DateTime startTime, List<Guid> participantIds, List<Guid> winnerIds, TournamentStatus status, bool isDraw)
        {
            Id = id;
            StartTime = startTime;
            ParticipantIds = participantIds;
            WinnerIds = winnerIds;
            Status = status;
            IsDraw = isDraw;
        }
    }

}
