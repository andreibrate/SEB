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
        public ScoreEP(UserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "GET" && request.Path == "score")
            {
                return HandleGetScoreboard(request, response);
            }

            return false;
        }

        private bool HandleGetScoreboard(HttpRequest request, HttpResponse response)
        {
            var users = _userHandler.GetAllUsers();

            var scoreboard = users
                .OrderByDescending(u => u.Elo)
                .Select(u => new
                {
                    Username = u.Username,
                    Elo = u.Elo,
                    TotalExercises = u.Exercises?.Count ?? 0
                })
                .ToList();

            response.Body = JsonSerializer.Serialize(scoreboard);
            response.Headers["Content-Type"] = "application/json";
            response.ResponseCode = 200;
            response.ResponseMessage = "OK";

            return true;
        }
    }
}
