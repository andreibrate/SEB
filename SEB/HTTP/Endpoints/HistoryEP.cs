using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SEB.BusinessLogic;
using SEB.Models;
using SEB.Models.Enums;

namespace SEB.HTTP.Endpoints
{
    public class HistoryEP : IEndpoint
    {
        private readonly ExerciseHandler _exerciseHandler;
        private readonly UserHandler _userHandler;
        private readonly TournamentHandler _tournamentHandler;

        public HistoryEP(ExerciseHandler exerciseHandler, UserHandler userHandler, TournamentHandler tournamentHandler)
        {
            _exerciseHandler = exerciseHandler;
            _userHandler = userHandler;
            _tournamentHandler = tournamentHandler;
        }

        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            var path = request.Path.TrimStart('/');

            if (request.Method == "GET" && path == "history")
                return HandleGetHistory(request, response);
            if (request.Method == "POST" && path == "history")
                return HandlePostHistory(request, response);

            return false;
        }


        private bool HandleGetHistory(HttpRequest request, HttpResponse response)
        {
            if (!request.Headers.ContainsKey("Authorization") || !request.Headers["Authorization"].StartsWith("Basic "))
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Unauthorized - Missing or invalid token";
                return true;
            }

            string token = request.Headers["Authorization"].Substring("Basic ".Length);
            var user = _userHandler.GetUserByToken(token);
            if (user == null)
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Unauthorized - Invalid token";
                return true;
            }

            // Fetch exercises for this user
            var exercises = _exerciseHandler.GetExercisesByUserId(user.Id);

            var history = exercises.Select(e => new
            {
                ExerciseType = e.Type.ToString(),
                Count = e.Count,
                Duration = e.Duration
            }).ToList();

            response.Body = JsonSerializer.Serialize(history);
            //response.Headers["Content-Type"] = "application/json";
            response.ResponseCode = 200;
            response.ResponseMessage = "OK";

            return true;
        }

        private bool HandlePostHistory(HttpRequest request, HttpResponse response)
        {
            if (!request.Headers.ContainsKey("Authorization") || !request.Headers["Authorization"].StartsWith("Basic "))
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Unauthorized - Missing or invalid token";
                return true;
            }

            string token = request.Headers["Authorization"].Substring("Basic ".Length);
            var user = _userHandler.GetUserByToken(token);
            if (user == null)
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Unauthorized - Invalid token";
                return true;
            }

            if (string.IsNullOrEmpty(request.Body))
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request - Missing body";
                return true;
            }

            try
            {
                // make both enum values and curl json lowercase (e.g. "pushups")
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true) }
                };

                // so that both exercise types (e.g. PushUps and pushups) are accepted
                var exercise = JsonSerializer.Deserialize<Exercise>(request.Body, options);

                if (exercise == null || exercise.Count <= 0 || exercise.Duration <= 0)
                {
                    response.ResponseCode = 400;
                    response.ResponseMessage = "Bad Request - Invalid exercise data";
                    return true;
                }

                exercise.Id = Guid.NewGuid();
                exercise.UserId = user.Id;

                _exerciseHandler.AddExercise(exercise);

                var tournaments = _tournamentHandler.GetAllTournaments();
                // existing = NotYetStarted or Active
                var existingTournament = tournaments.FirstOrDefault(t => t.Status == TournamentStatus.NotYetStarted || t.Status == TournamentStatus.Active);

                // tournament exists
                if (existingTournament != null)
                {
                    var participants = _tournamentHandler.GetParticipants(existingTournament.Id);

                    if (!participants.Contains(user.Id))
                    {
                        // for debugging
                        //Console.WriteLine($"Participants count: {participants.Count}");
                        _tournamentHandler.AddParticipant(existingTournament.Id, user.Id, exercise.Id);
                        Console.WriteLine($"User {user.Username} added to Tournament {existingTournament.Id}");

                        var updatedParticipants = _tournamentHandler.GetParticipants(existingTournament.Id);
                        // for debugging
                        //Console.WriteLine($"Updated participants count: {updatedParticipants.Count}");

                        // if 2 participants and tournament was NotYetStarted, activate it
                        if (updatedParticipants.Count == 2 && existingTournament.Status == TournamentStatus.NotYetStarted)
                        {
                            _tournamentHandler.UpdateStatus(existingTournament.Id, TournamentStatus.Active);
                            Console.WriteLine($"Tournament {existingTournament.Id} started.");
                        }
                    }
                }
                else
                {
                    // no tournament exists yet => create a new one and add user
                    var newTournament = _tournamentHandler.CreateTournament(user.Id, exercise.Id);

                    // for debugging
                    //Console.WriteLine($"Tournament {newTournament.Id} created with first user {user.Username}");
                }

                response.ResponseCode = 201;
                response.ResponseMessage = "Exercise added";
                return true;
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization failed: {ex.Message}");
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request - Malformed JSON";
                return true;
            }
        }

    }
}
