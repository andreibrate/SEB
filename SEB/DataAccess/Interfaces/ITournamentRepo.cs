using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.Models;

namespace SEB.DataAccess.Interfaces
{
    public interface ITournamentRepo
    {
        void CreateTournament(Tournament tournament);
        Tournament? GetTournamentById(Guid tournamentId);
        List<Tournament> GetAllTournaments();
        void SetWinner(Guid tournamentId, Guid winnerId, bool isDraw);
    }
}
