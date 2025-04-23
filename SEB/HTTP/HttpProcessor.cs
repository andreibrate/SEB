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

            // endpoints processing
            var pathSegments = request.Path.Split('/');
            var endpointKey = pathSegments.Length > 1 ? pathSegments[1] : string.Empty;
            var endpoint = httpServer.Endpoints.ContainsKey(endpointKey) ? httpServer.Endpoints[endpointKey] : null;
            if (endpoint == null || !endpoint.HandleRequest(request, response))
            {
                response.ResponseCode = 404;
                response.ResponseMessage = "Not Found";
                response.Body = "Not found!";
            }

            Console.WriteLine("----------------------------------------");

            // ----- 3. Write the HTTP-Response -----
            response.SendResponse();

            Console.WriteLine("========================================");
        }
    }
}
