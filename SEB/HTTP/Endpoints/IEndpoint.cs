using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEB.HTTP.Endpoints
{
    internal interface IEndpoint
    {
        bool HandleRequest(HttpRequest request, HttpResponse response);
    }
}
