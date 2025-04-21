using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SEB.HTTP
{
    internal class HttpProcessor
    {
        private TcpClient clientSocket;
        private HttpServer httpServer;
        public HttpProcessor(HttpServer httpServer, TcpClient clientSocket)
        {
            this.httpServer = httpServer;
            this.clientSocket = clientSocket;
        }

        public void Process()
        {
            // ----- 1. Read the HTTP-Request -----
            using var reader = new StreamReader(clientSocket.GetStream());
            var request = new HttpRequest(reader);
            request.ReadRequest();

            // ----- 2. Do the processing -----
            using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
            var response = new HttpResponse(writer);

            // endpoints - TBI

            // if (request.Method == "GET")
            // {
            //     response.ResponseCode = 200;
            //     response.ResponseMessage = "OK";
            //     response.Headers["Content-Type"] = "text/html";
            //     response.Body = "<html><body><h1>Hello World!</h1></body></html>";
            // }
            // else if (request.Method == "POST")
            // {
            //     response.ResponseCode = 200;
            //     response.ResponseMessage = "OK";
            //     response.Headers["Content-Type"] = "text/plain";
            //     response.Body = "POST request received!";
            // }
            // else
            // {
            //     response.ResponseCode = 405;
            //     response.ResponseMessage = "Method Not Allowed";
            //     response.Headers["Content-Type"] = "text/plain";
            //     response.Body = "Method Not Allowed";
            // }



            Console.WriteLine("----------------------------------------");
            // ----- 3. Write the HTTP-Response -----
            response.SendResponse();

            Console.WriteLine("========================================");
        }
    }
}
