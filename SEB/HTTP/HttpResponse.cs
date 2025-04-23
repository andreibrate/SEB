using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SEB.HTTP
{
    public class HttpResponse
    {
        private StreamWriter writer;

        public string HttpVersion { get; set; } = "HTTP/1.0";
        public int ResponseCode { get; set; } = 200;
        public string ResponseMessage { get; set; } = "OK";
        public Dictionary<string, string> Headers { get; set; } = new();
        public string? Body { get; set; }

        public HttpResponse(StreamWriter writer)
        {
            this.writer = writer;
        }

        public void SendResponse()
        {
            // we use a simple helper-class StreamTracer to write the HTTP-Response to the client and to the console
            var writerAlsoToConsole = new StreamTracer(writer);

            // first line in HTTP-Response contains the HTTP-Version and the status code
            writerAlsoToConsole.WriteLine($"{HttpVersion} {ResponseCode} {ResponseMessage}");

            // length of the content (= Body)
            if (Body != null)
            {
                Headers["Content-Length"] = Body.Length.ToString();
            }

            // the HTTP-headers (in HTTP after the first line, until the empty line)
            if (!Headers.ContainsKey("Content-Type"))
            {
                Headers["Content-Type"] = "text/html; charset=utf-8";
            }

            // the HTTP-headers (in HTTP after the first line, until the empty line)
            foreach (var header in Headers)
            {
                writerAlsoToConsole.WriteLine($"{header.Key}: {header.Value}");
            }

            // empty line indicates the end of the HTTP-headers
            writerAlsoToConsole.WriteLine();

            // the HTTP-content (here we just return a minimalistic HTML Hello-World)
            if (Body != null)
            {
                writerAlsoToConsole.WriteLine(Body);
            }
        }
    }
}
