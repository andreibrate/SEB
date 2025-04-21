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
        public string Body { get; set; }

        public HttpResponse(StreamWriter writer)
        {
            this.writer = writer;
        }

        public void SendResponse()
        {
            var writerAlsoToConsole = new StreamTracer(writer);

            writerAlsoToConsole.WriteLine($"{HttpVersion} {ResponseCode} {ResponseMessage}");
            if (Body != null)
            {
                Headers["Content-Length"] = Body.Length.ToString();
            }
            foreach (var header in Headers)
            {
                writerAlsoToConsole.WriteLine($"{header.Key}: {header.Value}");
            }
            writerAlsoToConsole.WriteLine(); // empty line indicates the end of the HTTP-headers
            if (Body != null)
            {
                writerAlsoToConsole.WriteLine(Body);
            }
        }
    }
}
