using Newtonsoft.Json;
using System.Text;
using TechDispo.Models;
using TechDispoB.Models;

namespace TechDispoB.Services
{
    public class AppService : IAppService
    {
        private readonly HttpClient _httpClient;

        public AppService()
        {
            _httpClient = HttpClientService.CreateHttpClient();
        }

        private async Task<T?> SendRequestAsync<T>(HttpMethod method, string url, object? body = null)
        {
            try
            {
                using var request = new HttpRequestMessage(method, url);

                if (body != null)
                {
                    var json = JsonConvert.SerializeObject(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la requête {method} vers {url}: {ex.Message}");
                return default;
            }
        }

        public async Task<string?> Login(LoginModel loginModel)
        {
            // Envoyer les données du modèle au serveur via une requête POST
            var result = await SendRequestAsync<string>(
                HttpMethod.Post,

                Apis.Login,  // Endpoint défini dans votre API
                loginModel
            );

            return result; // Renvoie le token ou un message en fonction de l'API
        }

        public async Task<List<Mission>> GetMissions()
        {
            var result = await SendRequestAsync<List<Mission>>(HttpMethod.Get, Apis.ListMissions);
            return result ?? new List<Mission>();
        }

        public async Task<Mission> GetMissionById(int missionId)
        {
            var result = await SendRequestAsync<Mission>(HttpMethod.Get, $"/api/mission/mission/{missionId}");
            return result ?? new Mission();
        }

        public async Task<bool> CanConnectToDatabase()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/connectdatabase");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
