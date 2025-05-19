using System.Net.Http.Headers;

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

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://2098-2a01-e0a-1d4-b530-ed35-8324-7d21-92ab.ngrok-free.app/api/")
            };

            // ✅ Ajouter le token JWT s’il existe
            var token = Preferences.Get("auth_token", string.Empty);
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }
    }

}
