using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechDispo.Models;
using TechDispoB.Models;  


namespace TechDispoB.Services.Implementations
{
    public class AppService : IAppService
    {
        private readonly HttpClient _httpClient;
        private JsonSerializerOptions? _jsonOptions;

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
                    var json = JsonSerializer.Serialize(body);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                }

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();

                // Gestion des erreurs de désérialisation
                try
                {
                    return JsonSerializer.Deserialize<T>(responseString, _jsonOptions);
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"Erreur de désérialisation : {jsonEx.Message}");
                    return default;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la requête {method} vers {url}: {ex.Message}");
                return default;
            }
        }


        public async Task<LoginResponse?> Login(LoginModel loginModel)
        {
            try
            {
                // Envoyer la requête POST avec un corps JSON
                var response = await _httpClient.PostAsJsonAsync("/auth/login", loginModel);
                Console.WriteLine(response);
                Console.WriteLine($"Email: {loginModel.Email}, Password: {loginModel.Password}, RememberMe: {loginModel.RememberMe}");
                var json = JsonSerializer.Serialize(loginModel);
                Console.WriteLine($"JSON envoyé : {json}");

                // Vérification si le status de la réponse est un succès
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Échec de la connexion : {response.StatusCode}, Contenu de la réponse : {errorContent}");
                    return null;
                }

                // Désérialisation de la réponse JSON en LoginResponse
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Pour gérer les différences de casse dans les noms de propriétés
                });

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Stocker le jeton dans SecureStorage
                    await SecureStorage.SetAsync("auth_token", loginResponse.Token);
                    Console.WriteLine("Jeton JWT stocké avec succès !");
                }

                return loginResponse;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la connexion : {ex.Message}");
                return null;
            }
        }

        public async Task<List<MissionDto>> GetMissions()
        {
            return await _httpClient.GetFromJsonAsync<List<MissionDto>>(Apis.ListMissions) ?? new List<MissionDto>();
        }
        public async Task<MissionDto> GetMissionById(int missionId)
        {
            var result = await SendRequestAsync<MissionDto>(HttpMethod.Get, $"/api/mission/mission/{missionId}");
            return result ?? new MissionDto();
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
