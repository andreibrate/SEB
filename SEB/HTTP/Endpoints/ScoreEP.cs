using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SEB.BusinessLogic;

namespace SEB.HTTP.Endpoints
{
    public class ScoreEP : IEndpoint
    {
        private readonly UserHandler _userHandler;
        private readonly ExerciseHandler _exerciseHandler;
        public ScoreEP(UserHandler userHandler, ExerciseHandler exerciseHandler)
        {
            _userHandler = userHandler;
            _exerciseHandler = exerciseHandler;
        }

        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "GET" && request.Path.TrimStart('/') == "score")
            {
                return HandleGetScoreboard(request, response);
            }

            return false;
        }

        private bool HandleGetScoreboard(HttpRequest request, HttpResponse response)
        {
            var users = _userHandler.GetAllUsers();
            var scoreboard = new List<object>();

            foreach (var user in users)
            {
                var exercises = _exerciseHandler.GetExercisesByUserId(user.Id);
                int total = exercises.Sum(e => e.Count);

                scoreboard.Add(new
                {
                    Username = user.Username,
                    Elo = user.Elo,
                    TotalExercises = total,
                    Rank = user.Rank.ToString()
                });
            }

            // dynamic used for anonymous object scoreboard, since it is not known if "u" has "Elo" property; used to avoid a compile-time error
            // alternative would be to create a DTO ()data transfer object) class for the scoreboard, like this:
            //public class ScoreboardEntry
            //{
            //    public string Username { get; set; } = "";
            //    public int Elo { get; set; }
            //    public int TotalExercises { get; set; }
            //}

            // sort by elo descending
            scoreboard = scoreboard.OrderByDescending(u => ((dynamic)u).Elo).ToList();

            response.Body = JsonSerializer.Serialize(scoreboard);
            //response.Headers["Content-Type"] = "application/json";
            response.ResponseCode = 200;
            response.ResponseMessage = "OK";
            return true;
        }

    }
}
