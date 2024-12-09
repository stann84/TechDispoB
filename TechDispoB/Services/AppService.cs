using Newtonsoft.Json;
using System.Text;
using TechDispo.Models;
using TechDispoB.Model;
using TechDispoB.Models;

namespace TechDispoB.Services
{
    public class AppService : IAppService
    {
        private readonly HttpClient _httpClient;

        public AppService()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://techdispoweb.azurewebsites.net")
            };
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

        public async Task<string> Login(LoginModel loginModel)
        {
            var result = await SendRequestAsync<string>(HttpMethod.Post, Apis.Login, loginModel);
            return result ?? "Échec de la connexion";
        }

        public async Task<List<Mission>> GetMissions()
        {
            var result = await SendRequestAsync<List<Mission>>(HttpMethod.Get, Apis.ListMissions);
            return result ?? new List<Mission>();
        }

        public async Task<Mission> GetMissionById(int missionId)
        {
            var result = await SendRequestAsync<Mission>(HttpMethod.Get, $"/api/Mission/mission/{missionId}");
            return result ?? new Mission();
        }

        public async Task<bool> CanConnectToDatabase()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/login");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
