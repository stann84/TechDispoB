using System.Net.Http.Json;
using System.Text.Json;
using TechDispoB.Models;

namespace TechDispoB.Services.Implementations
{
    public class AppService : IAppService
    {
        private readonly HttpClient _httpClient;

        public event Action? OnAuthStateChanged; // ✅ Ajout de l'événement
        public AppService()
        {
            _httpClient = HttpClientService.CreateHttpClient();
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
        public async Task<bool> IsAuthenticated()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token); // Retourne true si un token est stocké
        }
        public async Task<LoginResponseDto?> Login(LoginDto loginModel)
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

                // Désérialisation de la réponse JSON en LoginResponseDto
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true // Pour gérer les différences de casse dans les noms de propriétés
                });

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    // Stocker le jeton dans SecureStorage
                    await SecureStorage.SetAsync("auth_token", loginResponse.Token);
                    await SecureStorage.SetAsync("userId", loginResponse.User.Id);
                    Console.WriteLine("Jeton JWT stocké avec succès !");
                    Console.WriteLine($"user Id = : {loginResponse.User.Id}");
                    OnAuthStateChanged?.Invoke(); // Notifie Blazor
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
            return await _httpClient.GetFromJsonAsync<List<MissionDto>>(Apis.ListMissions) ?? [];
        }
        public async Task<MissionDto> GetMissionById(int missionId)
        {
            return await _httpClient.GetFromJsonAsync<MissionDto>($"/api/mission/{missionId}") ?? new MissionDto();
        }

        public async Task<List<MissionDto>> GetMissionsForUserAsync(string userId)
        {
            return await _httpClient.GetFromJsonAsync<List<MissionDto>>($"/api/mission/user/{userId}") ?? new List<MissionDto>();
        }
        public async Task Logout()
        {
            await SecureStorage.SetAsync("auth_token", ""); // Efface le token
            Console.WriteLine("Utilisateur déconnecté !");
            OnAuthStateChanged?.Invoke(); // Notifie Blazor
            
        }

    }
}
