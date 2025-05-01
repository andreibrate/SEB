using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SEB.HTTP;
using SEB.HTTP.Endpoints;

namespace SEB.HTTP
{
    internal class HttpServer
    {
        private IPAddress ip = IPAddress.Loopback; // localhost
        private int port = 10001;
        private TcpListener httpServer;

        // volatile = may be changed by other threads, so its value isn’t optimized or cached
        private volatile bool isRunning = true;
        public Dictionary<string, IEndpoint> Endpoints { get; } = new();

        public HttpServer()
        {
            this.httpServer = new TcpListener(ip, port);
        }

        public HttpServer(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.httpServer = new TcpListener(ip, port);
        }

        public void Run()
        {
            // ===== I. Start the HTTP-Server =====
            httpServer.Start();
            Console.WriteLine($"HTTP server started on {ip}:{port}");

            while (isRunning)
            {
                // Checks whether there’s a client trying to connect before calling AcceptTcpClient()
                if (httpServer.Pending())
                {
                    // ----- 0. Accept the TCP-Client and create the reader and writer -----
                    var clientSocket = httpServer.AcceptTcpClient();
                    var httpProcessor = new HttpProcessor(this, clientSocket);

                    // ThreadPool for multiple threads
                    ThreadPool.QueueUserWorkItem(o => httpProcessor.Process());
                }
                Thread.Sleep(100); // reduce CPU load
            }

            // cleanup code after the loop ends to free port and shut socket down,
            // not actually called again from Run()
            httpServer.Stop();
            Console.WriteLine("HTTP server stopped.");
        }

        public void RegisterEndpoint(string path, IEndpoint endpoint)
        {
            Endpoints.Add(path, endpoint);
        }

        public void Stop()
        {
            isRunning = false;
        }
    }
}
