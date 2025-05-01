using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SEB.BusinessLogic;
using SEB.Models;
using SEB.Models.Enums;

namespace SEB.HTTP.Endpoints
{
    public class ExerciseEP : IEndpoint
    {
        private readonly ExerciseHandler _exerciseHandler;
        private readonly UserHandler _userHandler;
        public ExerciseEP(ExerciseHandler exerciseHandler, UserHandler userHandler)
        {
            _exerciseHandler = exerciseHandler;
            _userHandler = userHandler;
        }

        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "POST" && request.Path.TrimStart('/') == "exercises")
            {
                return HandlePostExercise(request, response);
            }
            else if (request.Method == "GET" && request.Path.TrimStart('/') == "exercises")
            {
                return HandleGetExercises(request, response);
            }

            return false;
        }

        private bool HandlePostExercise(HttpRequest request, HttpResponse response)
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

            if (string.IsNullOrWhiteSpace(request.Body))
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request - No content provided";
                return true;
            }

            try
            {
                var exercise = JsonSerializer.Deserialize<Exercise>(request.Body);
                if (exercise == null || exercise.Count <= 0 || exercise.Duration <= 0)
                {
                    response.ResponseCode = 400;
                    response.ResponseMessage = "Bad Request - Invalid exercise data";
                    return true;
                }

                exercise.UserId = user.Id;
                exercise.Id = Guid.NewGuid();

                _exerciseHandler.AddExercise(exercise);

                response.ResponseCode = 201;
                response.ResponseMessage = "Exercise added successfully";
            }
            catch
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request - Malformed JSON";
            }

            return true;
        }

        private bool HandleGetExercises(HttpRequest request, HttpResponse response)
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

            var exercises = _exerciseHandler.GetExercisesByUserId(user.Id);

            response.Body = JsonSerializer.Serialize(exercises);
            response.ResponseCode = 200;
            response.ResponseMessage = "OK";
            return true;
        }
    }
}
