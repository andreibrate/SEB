using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.DataAccess.Interfaces;
using SEB.Models;

namespace SEB.BusinessLogic
{
    public class TournamentHandler
    {
        private readonly ITournamentRepo _tournamentRepo;
        private readonly IExerciseRepo _exerciseRepo;
        private readonly IUserRepo _userRepo;
        public TournamentHandler(ITournamentRepo tournamentRepo, IExerciseRepo exerciseRepo, IUserRepo userRepo)
        {
            _tournamentRepo = tournamentRepo;
            _exerciseRepo = exerciseRepo;
            _userRepo = userRepo;
        }

        public Tournament CreateTournament(List<Guid> participantIds)
        {
            var tournament = new Tournament(participantIds);
            _tournamentRepo.CreateTournament(tournament);

            foreach (var userId in participantIds)
            {
                _tournamentRepo.AddParticipant(tournament.Id, userId);
            }

            return tournament;
        }

        public void FinishTournament(Guid tournamentId)
        {
            var tournament = _tournamentRepo.GetTournamentById(tournamentId);
            var participants = _tournamentRepo.GetParticipants(tournamentId);
            var exerciseResults = new Dictionary<Guid, int>(); // UserId -> exercise count

            // Sum all exercises for each participant
            foreach (var userId in participants)
            {
                var exercises = _exerciseRepo.GetExercisesByUserId(userId);
                int totalExercises = exercises.Sum(e => e.Count);
                exerciseResults[userId] = totalExercises;
            }

            int maxExercises = exerciseResults.Values.Max();
            var winners = exerciseResults
                .Where(x => x.Value == maxExercises)    // exercise count
                .Select(x => x.Key)                     // userId
                .ToList();

            bool isDraw = winners.Count > 1;

            _tournamentRepo.SetWinners(tournamentId, winners, isDraw);

            // Update ELO based on results
            foreach (var userId in participants)
            {
                var user = _userRepo.GetUserById(userId);
                if (user == null)
                {
                    Console.WriteLine($"Warning: User {userId} not found during tournament finish.");
                    continue;
                }

                if (winners.Contains(userId))
                {
                    user.Elo += isDraw ? 1 : 2;
                }
                else
                {
                    user.Elo -= 1;
                }

                _userRepo.UpdateUser(user);
            }
        }


    }
}
