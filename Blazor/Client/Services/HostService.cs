using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Blazor.Client
{
    public class HostService
    {
        public readonly HttpClient httpClient;

        public HostService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
    }
}
