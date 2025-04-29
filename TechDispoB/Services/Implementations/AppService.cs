using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using TechDispoB.Models;

namespace TechDispoB.Services.Implementations
{
    public class AppService : IAppService
    {
        private readonly HttpClient _httpClient;

        public event Action? OnAuthStateChanged;

        public AppService()
        {
            _httpClient = HttpClientService.CreateHttpClient();
        }

        // 📡 Vérifie si la base de données est accessible
        public async Task<bool> CanConnectToDatabase()
        {
            try
            {
                var response = await _httpClient.GetAsync(Apis.Users.CheckDatabaseConnection);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        // 🔐 Vérifie si l'utilisateur est connecté
        public async Task<bool> IsAuthenticated()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }

        // 🌐 Générique : GET API et désérialisation automatique
        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❗ Erreur API GET {url} : {response.StatusCode} - {errorContent}");
                    return default;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Exception GET {url} : {ex.Message}");
                return default;
            }
        }

        // 🌐 Générique : POST API avec données
        public async Task<bool> PostAsync<T>(string url, T data)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❗ Erreur API POST {url} : {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Exception POST {url} : {ex.Message}");
                return false;
            }
        }

        // 🌐 Générique : PUT API avec données
        public async Task<bool> PutAsync<T>(string url, T data)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❗ Erreur API PUT {url} : {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Exception PUT {url} : {ex.Message}");
                return false;
            }
        }

        // 🌐 Générique : POST API sans contenu
        public async Task<bool> PostWithoutDataAsync(string url)
        {
            try
            {
                var response = await _httpClient.PostAsync(url, null);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❗ Erreur API POST (vide) {url} : {response.StatusCode} - {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Exception POST (vide) {url} : {ex.Message}");
                return false;
            }
        }

        // 🛡️ Connexion d'un utilisateur
        public async Task<LoginResponseDto?> Login(LoginDto loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(Apis.Users.Login, loginModel);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Échec de la connexion : {response.StatusCode}, Contenu : {errorContent}");
                    return null;
                }

                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    await SecureStorage.SetAsync("auth_token", loginResponse.Token);
                    await SecureStorage.SetAsync("userId", loginResponse.User.Id);
                    Console.WriteLine("✅ Jeton JWT stocké !");
                    OnAuthStateChanged?.Invoke();
                }

                return loginResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la connexion : {ex.Message}");
                return null;
            }
        }

        // 🔐 Déconnexion
        public async Task Logout()
        {
            await SecureStorage.SetAsync("auth_token", "");
            Console.WriteLine("Utilisateur déconnecté !");
            OnAuthStateChanged?.Invoke();
        }

        // 🔥 Envoie du FCM Token
        public async Task<bool> SendFCMTokenAsync(string fcmToken, string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(fcmToken))
                    return false;

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

                var data = new UserDto { FCMToken = fcmToken };
                return await PostAsync(Apis.Users.UpdateFCMToken, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'envoi du token FCM : {ex.Message}");
                return false;
            }
        }

        // 🔎 Récupère un utilisateur par son ID
        public async Task<UserDto> GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            var url = $"{Apis.Users.GetById}/{userId}";
            return await GetAsync<UserDto>(url);
        }

        // 📍 Mise à jour de la localisation de l'utilisateur
        public async Task<bool> UpdateUserLocationAsync(string userId, double latitude, double longitude)
        {
            var url = $"{Apis.Users.UpdateLocation}/{userId}/location";
            var data = new { Latitude = latitude, Longitude = longitude };

            return await PutAsync(url, data);
        }

        // 🚀 Récupération des missions
        public async Task<List<MissionDto>> GetMissions()
        {
            return await GetAsync<List<MissionDto>>(Apis.Missions.List) ?? [];
        }

        // 🔎 Récupération d'une mission par ID
        public async Task<MissionDto> GetMissionById(int missionId)
        {
            return await GetAsync<MissionDto>($"/api/mission/{missionId}") ?? new MissionDto();
        }

        // 🚀 Missions attribuées à un utilisateur
        public async Task<List<MissionDto>> GetMissionsForUserAsync(string userId)
        {
            var url = $"{Apis.Missions.GetByUser}/{userId}";
            return await GetAsync<List<MissionDto>>(url) ?? new List<MissionDto>();
        }

        // 📥 Accepter une mission
        public async Task<bool> AccepterMission(int missionId)
        {
            var url = string.Format(Apis.Missions.Accept, missionId);
            return await PostWithoutDataAsync(url);
        }

        // 📤 Refuser une mission
        public async Task<bool> RefuserMission(int missionId)
        {
            var url = string.Format(Apis.Missions.Refuse, missionId);
            return await PostWithoutDataAsync(url);
        }

        // 🚦 Commencer une mission
        public async Task<bool> CommencerMission(int missionId)
        {
            var url = string.Format(Apis.Missions.Commencer, missionId);
            return await PostWithoutDataAsync(url);
        }

        // 🏁 Clôturer une mission
        public async Task<bool> CloturerMission(int missionId)
        {
            var url = string.Format(Apis.Missions.Cloturer, missionId);
            return await PostWithoutDataAsync(url);
        }
    }
}
