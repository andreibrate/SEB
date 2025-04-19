using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SEB.HTTP;

namespace SEB.HTTP
{
    internal class HttpServer
    {
        private IPAddress ip = IPAddress.Loopback; // localhost
        private int port = 10001;
        private TcpListener httpServer;

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

            while (true)
            {
                // ----- 0. Accept the TCP-Client and create the reader and writer -----
                var clientSocket = httpServer.AcceptTcpClient();
                //var httpProcessor = new HttpProcessor(this, clientSocket);
                // ThreadPool for multiple threads
                //ThreadPool.QueueUserWorkItem(o => httpProcessor.Process());
                using var writer = new StreamWriter(clientSocket.GetStream()) { AutoFlush = true };
                using var reader = new StreamReader(clientSocket.GetStream());

                // ----- 1. Read the HTTP-Request -----
                string? line;

                

                // ----- 2. Do the processing -----
                // .... 

                Console.WriteLine("----------------------------------------");

                // ----- 3. Write the HTTP-Response -----
                var writerAlsoToConsole = new StreamTracer(writer);  // we use a simple helper-class StreamTracer to write the HTTP-Response to the client and to the console

                writerAlsoToConsole.WriteLine("HTTP/1.0 200 OK");    // first line in HTTP-Response contains the HTTP-Version and the status code
                writerAlsoToConsole.WriteLine("Content-Type: text/html; charset=utf-8");     // the HTTP-headers (in HTTP after the first line, until the empy line)
                writerAlsoToConsole.WriteLine();
                writerAlsoToConsole.WriteLine("<html><body><h1>Hello World!</h1></body></html>");    // the HTTP-content (here we just return a minimalistic HTML Hello-World)

                Console.WriteLine("========================================");

                Thread.Sleep(100); // reduce CPU load
            }
        }
    }
}
