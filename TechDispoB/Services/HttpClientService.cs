using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDispoB.Services
{
    public static class HttpClientService
    {
        public static HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri("https://be27-2a01-e0a-1d4-b530-3012-bc1-c882-58d8.ngrok-free.app")
            };
        }
    }
}
