using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SEB.BusinessLogic;
using SEB.Models;

namespace SEB.HTTP.Endpoints
{
    public class TournamentEP : IEndpoint
    {
        private readonly TournamentHandler _tournamentHandler;
        private readonly ExerciseHandler _exerciseHandler;
        private readonly UserHandler _userHandler;
        public TournamentEP(TournamentHandler tournamentHandler, UserHandler userHandler, ExerciseHandler exerciseHandler)
        {
            _tournamentHandler = tournamentHandler;
            _userHandler = userHandler;
            _exerciseHandler = exerciseHandler;
        }
        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "GET" && request.Path.TrimStart('/') == "tournament")
            {
                return HandleGetTournaments(request, response);
            }

            return false;
        }
        private bool HandleGetTournaments(HttpRequest request, HttpResponse response)
        {
            var tournaments = _tournamentHandler.GetAllTournaments();
            var tournamentInfos = new List<object>();

            foreach (var tournament in tournaments)
            {
                var participants = _tournamentHandler.GetParticipants(tournament.Id);
                string leaderUsername = "-";

                if (participants.Any())
                {
                    var bestUser = _tournamentHandler.GetCurrentLeader(participants);
                    if (bestUser != null)
                    {
                        leaderUsername = bestUser.Username;
                    }
                }

                tournamentInfos.Add(new
                {
                    Id = tournament.Id,
                    StartTime = tournament.StartTime,
                    Status = tournament.Status.ToString(),
                    ParticipantsCount = participants.Count,
                    Leader = leaderUsername,
                    IsDraw = tournament.IsDraw
                });
            }

            response.Body = JsonSerializer.Serialize(tournamentInfos);
            response.ResponseCode = 200;
            response.ResponseMessage = "OK";

            return true;
        }
    }
}
