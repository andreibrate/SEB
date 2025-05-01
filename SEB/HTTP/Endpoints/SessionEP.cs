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
    public class SessionEP : IEndpoint
    {
        private readonly UserHandler _userHandler;
        public SessionEP(UserHandler userHandler)
        {
            _userHandler = userHandler;
        }

        public bool HandleRequest(HttpRequest request, HttpResponse response)
        {
            if (request.Method == "POST" && request.Path.TrimStart('/') == "sessions")
            {
                return HandleLogin(request, response);
            }

            return false;
        }

        private bool HandleLogin(HttpRequest request, HttpResponse response)
        {
            if (string.IsNullOrWhiteSpace(request.Body))
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request - Missing login data";
                return true;
            }

            try
            {
                var loginData = JsonSerializer.Deserialize<User>(request.Body);
                if (loginData == null || string.IsNullOrEmpty(loginData.Username) || string.IsNullOrEmpty(loginData.Password))
                {
                    response.ResponseCode = 400;
                    response.ResponseMessage = "Bad Request - Invalid login data";
                    return true;
                }

                var user = _userHandler.Login(loginData.Username, loginData.Password);
                if (user == null)
                {
                    response.ResponseCode = 401;
                    response.ResponseMessage = "Unauthorized - Invalid username or password";
                    return true;
                }

                response.Body = JsonSerializer.Serialize(new
                {
                    Token = user.Token          // anonymous object
                });
                response.Headers["Content-Type"] = "application/json";
                response.ResponseCode = 200;
                response.ResponseMessage = "Login successful";
            }
            catch
            {
                response.ResponseCode = 400;
                response.ResponseMessage = "Bad Request - Malformed JSON";
            }

            return true;
        }
    }
}
