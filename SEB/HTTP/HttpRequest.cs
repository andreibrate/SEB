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

        public HttpRequest(StreamReader reader)
        {
            this.reader = reader;
        }

        public void ReadRequest()
        {
            // 1.1 first line in HTTP contains the method, path and HTTP version
            string? line = reader.ReadLine();
            if (line != null)
                Console.WriteLine(line);

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
                var parts = line.Split(':');
                if (parts.Length == 2 && parts[0] == "Content-Length")
                {
                    content_length = int.Parse(parts[1].Trim());
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
                Console.WriteLine(data.ToString());
            }
        }
    }
}
