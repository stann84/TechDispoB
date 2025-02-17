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
                BaseAddress = new Uri(" https://9089-2a01-e0a-1d4-b530-8ce3-69ac-1234-9577.ngrok-free.app/api/")
            };
        }
    }
}
