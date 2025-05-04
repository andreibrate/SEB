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
    public class StatsEP : IEndpoint
    {
        private readonly UserHandler _userHandler;
        private readonly ExerciseHandler _exerciseHandler;

        public StatsEP(UserHandler userHandler, ExerciseHandler exerciseHandler)
        {
            _userHandler = userHandler;
            _exerciseHandler = exerciseHandler;
        }

        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "GET" && request.Path.TrimStart('/') == "stats")
            {
                return HandleGetStats(request, response);
            }

            return false;
        }

        private bool HandleGetStats(HttpRequest request, HttpResponse response)
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

            // Calculate total push-ups (1 exercise = 1 pushup currently)
            var exercises = _exerciseHandler.GetExercisesByUserId(user.Id);
            int totalExercises = exercises.Sum(e => e.Count);

            var statsResult = new
            {
                Elo = user.Elo,
                TotalExercises = totalExercises
            };

            response.Body = JsonSerializer.Serialize(statsResult);
            //response.Headers["Content-Type"] = "application/json";
            response.ResponseCode = 200;
            response.ResponseMessage = "OK";

            return true;
        }
    }
}
