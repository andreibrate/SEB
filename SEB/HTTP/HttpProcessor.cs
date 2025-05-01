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
            // Console.WriteLine($"Incoming request path: {request.Path}"); // used previously for debugging
            var cleanPath = request.Path.TrimStart('/');
            // Console.WriteLine($"Cleaned path: {cleanPath}"); // used previously for debugging
            var pathSegments = cleanPath.Split('/');
            var endpointKey = pathSegments.Length > 0 ? pathSegments[0].Trim().ToLowerInvariant() : string.Empty;
            // Console.WriteLine($"Resolved endpointKey: '{endpointKey}'");  // used previously for debugging
            //var endpoint = httpServer.Endpoints.ContainsKey(endpointKey) ? httpServer.Endpoints[endpointKey] : null;
            //if (endpoint == null || !endpoint.HandleRequest(request, response))

            // keyword "out" allows TryGetValue to modify the endpoint variable directly (by reference)
            if (!httpServer.Endpoints.TryGetValue(endpointKey, out var endpoint) || !endpoint.HandleRequest(request, response))
            {
                // Console.WriteLine($"Endpoint not found: {endpointKey}");  // used previously for debugging
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
