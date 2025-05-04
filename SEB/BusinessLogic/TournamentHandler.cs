using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SEB.DataAccess.Interfaces;
using SEB.Models;
using SEB.Models.Enums;


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

        public Tournament CreateTournament(Guid userId, Guid exerciseId)
        {
            var tournament = new Tournament();
            _tournamentRepo.CreateTournament(tournament);
            _tournamentRepo.AddParticipant(tournament.Id, userId, exerciseId, 0);
            return tournament;
        }


        public void FinishTournament(Guid tournamentId)
        {
            var tournament = _tournamentRepo.GetTournamentById(tournamentId);
            var participants = _tournamentRepo.GetParticipants(tournamentId);
            var exerciseResults = new Dictionary<Guid, int>(); // UserId -> exercise count

            if (tournament == null)
            {
                throw new Exception("Tournament not found.");
            }

            // Sum all exercises for each participant
            foreach (var userId in participants)
            {
                var exercises = _exerciseRepo.GetExercisesByUserId(userId);
                int totalPoints = 0;

                foreach (var e in exercises)
                {
                    if (e.Duration <= 120)
                    {
                        int multiplier = e.Type switch
                        {
                            ExerciseTypes.PushUps => 1,
                            ExerciseTypes.Burpees => 2,
                            _ => 0
                        };
                        totalPoints += e.Count * multiplier;
                    }
                }
                exerciseResults[userId] = totalPoints;
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

        public List<Tournament> GetAllTournaments()
        {
            return _tournamentRepo.GetAllTournaments();
        }

        public List<Guid> GetParticipants(Guid tournamentId)
        {
            return _tournamentRepo.GetParticipants(tournamentId);
        }

        public User? GetCurrentLeader(List<Guid> participantIds)
        {
            var userScores = new Dictionary<User, int>();

            foreach (var userId in participantIds)
            {
                var user = _userRepo.GetUserById(userId);
                if (user != null)
                {
                    var exercises = _exerciseRepo.GetExercisesByUserId(userId);
                    int totalPushups = exercises.Sum(e => e.Count);
                    userScores[user] = totalPushups;
                }
            }

            return userScores
                .OrderByDescending(p => p.Value) // highest number of pushups
                .Select(p => p.Key)
                .FirstOrDefault();
        }

        public void AddParticipant(Guid tournamentId, Guid userId, Guid exerciseId)
        {
            _tournamentRepo.AddParticipant(tournamentId, userId, exerciseId, eloChange: 0);
        }

        public void UpdateStatus(Guid tournamentId, TournamentStatus status)
        {
            _tournamentRepo.UpdateStatus(tournamentId, status);
        }

    }
}
