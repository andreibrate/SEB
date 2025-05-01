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
    public class UserEP : IEndpoint
    {
        private readonly UserHandler _userHandler;
        public UserEP(UserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            var path = request.Path.TrimStart('/');

            if (request.Method == "POST" && path == "users")
            {
                return HandlePostUserRequests(request, response);
            }
            else if (request.Method == "GET" && path.StartsWith("users"))
            {
                return HandleGetUserRequests(request, response);
            }
            else if (request.Method == "PUT" && path.StartsWith("users"))
            {
                return HandlePutUserRequests(request, response);
            }

            return false;
        }

        private bool HandlePostUserRequests(HttpRequest request, HttpResponse response)
        {
            if (request.Body == null)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request";
                return true;
            }

            // Deserialize the JSON content into a User object
            var userData = JsonSerializer.Deserialize<User>(request.Body);
            if (userData == null || string.IsNullOrEmpty(userData.Username) || string.IsNullOrEmpty(userData.Password))
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request";
                return true;
            }

            // Register the user
            bool registrationSuccess;
            try
            {
                _userHandler.Register(userData);
                registrationSuccess = true;
            }
            catch
            {
                registrationSuccess = false;
            }

            if (registrationSuccess)
            {
                response.ResponseCode = 201;
                response.ResponseMessage = "OK";
            }
            else
            {
                response.ResponseCode = 409;
                response.ResponseMessage = "User already exists";
            }

            return true;
        }

        private bool HandleGetUserRequests(HttpRequest request, HttpResponse response)
        {
            if (!request.Headers.ContainsKey("Authorization") || !request.Headers["Authorization"].StartsWith("Basic "))
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Unauthorized - Missing or invalid token";
                return true;
            }

            string token = request.Headers["Authorization"].Substring("Basic ".Length);
            string requestedUsername = request.Path.Split('/').Last();

            // Find User by Token
            var requestingUser = _userHandler.GetUserByToken(token);
            if (requestingUser == null)
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Unauthorized - Invalid token";
                return true;
            }

            var user = _userHandler.GetUserByUsername(requestedUsername);

            if (user == null)
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "User not found";
                return true;
            }

            // Serialize the user object to JSON
            try
            {
                response.Body = JsonSerializer.Serialize(user);
                response.ResponseCode = 200;
                response.ResponseMessage = "OK";
            }
            catch (JsonException)
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Internal Server Error - JSON serialization failed";
            }

            return true;
        }

        private bool HandlePutUserRequests(HttpRequest request, HttpResponse response)
        {
            if (!request.Headers.ContainsKey("Authorization") || !request.Headers["Authorization"].StartsWith("Basic "))
            {
                response.ResponseCode = 401;
                response.ResponseMessage = "Unauthorized - Missing or invalid token";
                return true;
            }

            string token = request.Headers["Authorization"].Substring("Basic ".Length);
            string requestedUsername = request.Path.Split('/').Last();

            // Find User by Token
            var requestingUser = _userHandler.GetUserByToken(token);
            if (requestingUser == null || requestingUser.Username != requestedUsername)
            {
                response.ResponseCode = 403;
                response.ResponseMessage = "Forbidden - You can only access your own profile";
                return true;
            }

            if (request.Body == null)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request";
                return true;
            }

            // Deserialize the JSON content into a User object
            var userData = JsonSerializer.Deserialize<User>(request.Body);
            if (userData == null)
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request";
                return true;
            }

            var user = _userHandler.GetUserByUsername(requestedUsername);
            if (user == null)
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "User not found";
                return true;
            }

            user.Username = !string.IsNullOrEmpty(userData.Username) ? userData.Username : user.Username;
            user.Bio = !string.IsNullOrEmpty(userData.Bio) ? userData.Bio : user.Bio;
            user.Image = !string.IsNullOrEmpty(userData.Image) ? userData.Image : user.Image;

            // Update the user
            bool updateSuccess;
            try
            {
                _userHandler.UpdateUser(user);
                updateSuccess = true;
            }
            catch
            {
                updateSuccess = false;
            }
            if (updateSuccess)
            {
                response.ResponseCode = 200;
                response.ResponseMessage = "OK";
            }
            else
            {
                response.ResponseCode = 500;
                response.ResponseMessage = "Internal Server Error - User update failed";
            }

            return true;
        }
    }
}
