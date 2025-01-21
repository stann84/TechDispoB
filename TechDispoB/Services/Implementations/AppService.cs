using System.Net.Http.Json;
using System.Text.Json;
using TechDispoB.Models;
using TechDispoB.Services.Interfaces;

namespace TechDispoB.Services.Implementations
{
    public class AppService : IAppService
    {
        private readonly HttpClient _httpClient;

        public AppService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<LoginResponse?> Login(LoginModel loginModel)
        {
            try
            {
                Console.WriteLine($"Tentative de connexion : {loginModel.Email}");

                // 🔹 Envoyer la requête POST avec un JSON
                var response = await _httpClient.PostAsJsonAsync("/auth/login", loginModel);

                // 🔹 Vérification de la réponse HTTP
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Connexion échouée : {response.StatusCode}, Erreur : {errorContent}");
                    return null;
                }

                // 🔹 Désérialisation de la réponse JSON
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // 🔹 Stocker le token en SecureStorage
                    await SecureStorage.SetAsync("auth_token", loginResponse.Token);
                    Console.WriteLine("✅ Jeton JWT stocké avec succès !");
                }

                return loginResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erreur lors de la connexion : {ex.Message}");
                return null;
            }
        }

        public async Task<List<MissionDto>> GetMissions()
        {
            try
            {
                Console.WriteLine("📡 Récupération des missions...");
                return await _httpClient.GetFromJsonAsync<List<MissionDto>>(Apis.ListMissions) ?? new List<MissionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors du chargement des missions : {ex.Message}");
                return new List<MissionDto>();
            }
        }

        public async Task<MissionDto> GetMissionById(int missionId)
        {
            try
            {
                Console.WriteLine($"📡 Récupération de la mission ID {missionId}...");
                return await _httpClient.GetFromJsonAsync<MissionDto>($"/api/mission/mission/{missionId}") ?? new MissionDto();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de la récupération de la mission {missionId} : {ex.Message}");
                return new MissionDto();
            }
        }

        public async Task<bool> CanConnectToDatabase()
        {
            try
            {
                Console.WriteLine("📡 Vérification de la connexion à la base de données...");
                var response = await _httpClient.GetAsync("/api/connectdatabase");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Impossible de se connecter à la base de données : {ex.Message}");
                return false;
            }
        }
    }
}
