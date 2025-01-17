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
                BaseAddress = new Uri("https://36e2-2a01-e0a-1d4-b530-8124-9371-6bb9-da6a.ngrok-free.app")
            };

            Task.Run(async ()=> await AddAuthorizationHeader(client)).Wait(); 

            //return new HttpClient(handler)
            //{
            //    BaseAddress = new Uri("https://36e2-2a01-e0a-1d4-b530-8124-9371-6bb9-da6a.ngrok-free.app")
            //};
            return client;
        }
        private static async Task AddAuthorizationHeader(HttpClient client)
        {
            try
            {
                var token = await SecureStorage.GetAsync("token"); // Récupérer le token JWT

                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine("✅ Token ajouté aux requêtes HTTP !");
                }
                else
                {
                    Console.WriteLine("❌ Aucun token trouvé dans SecureStorage.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erreur lors de l'ajout du token : {ex.Message}");
            }
        }
    }
}
