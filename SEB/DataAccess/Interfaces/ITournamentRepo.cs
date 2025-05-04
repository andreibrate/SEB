using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.Models;
using SEB.Models.Enums;

namespace SEB.DataAccess.Interfaces
{
    public interface ITournamentRepo
    {
        void CreateTournament(Tournament tournament);
        Tournament? GetTournamentById(Guid tournamentId);
        List<Tournament> GetAllTournaments();
        void AddParticipant(Guid tournamentId, Guid userId, Guid exerciseId, int eloChange);
        List<Guid> GetParticipants(Guid tournamentId);
        void SetWinners(Guid tournamentId, List<Guid> winnerIds, bool isDraw);
        void UpdateStatus(Guid tournamentId, TournamentStatus status);
    }
}
