using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEB.HTTP
{
    public class HttpRequest
    {
        private StreamReader reader;

        public string Method { get; set; } = "";
        public string Path { get; set; } = "";
        public string HttpVersion { get; set; } = "";

        // create headers with case-insensitive keys
        // e.g. Headers["Authorization"] treated the same as Headers["authorization"]
        public Dictionary<string, string> Headers { get; } = new(StringComparer.OrdinalIgnoreCase);
        public string Body { get; set; } = "";
        public Dictionary<string, string>? RouteParameters { get; set; }

        public HttpRequest(StreamReader reader)
        {
            this.reader = reader;
        }

        public void ReadRequest()
        {
            // 1.1 first line in HTTP contains the method, path and HTTP version
            string? line = reader.ReadLine();
            if (line != null)
            {
                Console.WriteLine(line);
                var parts = line.Split(' ');
                if (parts.Length == 3)
                {
                    Method = parts[0].Trim();
                    Path = parts[1].Trim();
                    HttpVersion = parts[2].Trim();
                }
            }

            // 1.2 read the HTTP-headers (in HTTP after the first line, until the empy line)
            int content_length = 0; // we need the content_length later, to be able to read the HTTP-content
            while ((line = reader.ReadLine()) != null)
            {
                Console.WriteLine(line);
                if (line == "")
                {
                    break;  // empty line indicates the end of the HTTP-headers
                }

                // Parse the header
                var parts = line.Split(':', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();
                    Headers[key] = value;

                    // check if key is the same as content length, ignoring uppercase/lowercase differences
                    // if (key.ToLower() == "content-length") // simpler version
                    if (key.Equals("Content-Length", StringComparison.OrdinalIgnoreCase))
                    {
                        content_length = int.Parse(value);
                    }
                }
            }

            // 1.3 read the body if existing
            if (content_length > 0)
            {
                var data = new StringBuilder(200);
                char[] chars = new char[1024];
                int bytesReadTotal = 0;
                while (bytesReadTotal < content_length)
                {
                    var bytesRead = reader.Read(chars, 0, chars.Length);
                    bytesReadTotal += bytesRead;
                    if (bytesRead == 0)
                        break;
                    data.Append(chars, 0, bytesRead);
                }

                Body = data.ToString();
                Console.WriteLine(Body);
            }
        }
    }
}
