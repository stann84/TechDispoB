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
                BaseAddress = new Uri("https://4346-2a01-e0a-1d4-b530-8890-f598-b13d-da75.ngrok-free.app/api/")
            };
        }
    }
}
