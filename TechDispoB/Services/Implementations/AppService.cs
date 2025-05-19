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

        public AppService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // 🌐 Générique : Get  API avec données
        public async Task<T?> GetAsync<T>(string url)
        {
            try
            {
                var token = await SecureStorage.GetAsync("auth_token");

                var request = new HttpRequestMessage(HttpMethod.Get, url);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);

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
                var token = await SecureStorage.GetAsync("auth_token");

                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                };

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);

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
                var token = await SecureStorage.GetAsync("auth_token");

                var request = new HttpRequestMessage(HttpMethod.Put, url)
                {
                    Content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
                };

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);

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
                var token = await SecureStorage.GetAsync("auth_token");

                var request = new HttpRequestMessage(HttpMethod.Post, url);

                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await _httpClient.SendAsync(request);

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
            var url = string.Format(Apis.Missions.Accepter, missionId);
            return await PostWithoutDataAsync(url);
        }

        // 📤 Refuser une mission
        public async Task<bool> RefuserMission(int missionId)
        {
            var url = string.Format(Apis.Missions.Refuser, missionId);
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
