using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechDispoB.Models;  


namespace TechDispoB.Services.Implementations
{
    public class AppService : IAppService
    {
        private readonly HttpClient _httpClient;
        public async Task<List<MissionDto>> GetMissions()
        {
            return await _httpClient.GetFromJsonAsync<List<MissionDto>>(Apis.ListMissions) ?? new List<MissionDto>();
        }
        public async Task<MissionDto> GetMissionById(int missionId)
        {
            return await _httpClient.GetFromJsonAsync<MissionDto>($"/api/mission/mission/{missionId}") ?? new MissionDto();
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
        public async Task<LoginResponse?> Login(LoginModel loginModel)
        {
            try
            {
                // Envoyer la requête POST avec un corps JSON
                var response = await _httpClient.PostAsJsonAsync("/auth/login", loginModel);

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
        public async Task<bool> ValidateToken(string token)
        {
            try
            {
                // Ajouter le token dans les en-têtes de la requête
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("/auth/validate-token");

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la validation du token : {ex.Message}");
                return false;
            }
        }


    }
}
