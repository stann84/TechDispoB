namespace TechDispoB.Services.Implementations
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
                BaseAddress = new Uri("https://37e1-2a01-e0a-1d4-b530-3449-b02c-66f9-aec1.ngrok-free.app/api/")
            };
        }
    }
}
