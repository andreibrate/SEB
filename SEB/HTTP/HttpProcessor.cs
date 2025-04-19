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
            // TBI:
            // var response = new HttpResponse(writer);
        }
    }
}
