using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SEB.BusinessLogic;

namespace SEB.HTTP.Endpoints
{
    public class HistoryEP : IEndpoint
    {
        private readonly ExerciseHandler _exerciseHandler;
        private readonly UserHandler _userHandler;

        public HistoryEP(ExerciseHandler exerciseHandler, UserHandler userHandler)
        {
            _exerciseHandler = exerciseHandler;
            _userHandler = userHandler;
        }

        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "GET" && request.Path.TrimStart('/') == "history")
            {
                return HandleGetHistory(request, response);
            }

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
            response.Headers["Content-Type"] = "application/json";
            response.ResponseCode = 200;
            response.ResponseMessage = "OK";

            return true;
        }
    }
}
