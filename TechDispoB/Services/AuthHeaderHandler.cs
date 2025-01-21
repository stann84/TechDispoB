using System.Net.Http.Headers;

namespace TechDispoB.Services
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token"); // 🔹 Récupérer le token JWT

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    Console.WriteLine("✅ Token ajouté à la requête HTTP !");
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

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
