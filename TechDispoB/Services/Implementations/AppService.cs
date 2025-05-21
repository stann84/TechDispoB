using System.Net.Http.Json;
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

        // 🔐 Injection du token JWT pour chaque requête
        private async Task<HttpRequestMessage> CreateRequest(HttpMethod method, string url, object? body = null)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            var request = new HttpRequestMessage(method, url);

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            if (body != null)
            {
                var json = JsonSerializer.Serialize(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return request;
        }

        // 🔄 Méthodes HTTP génériques
        private async Task<T?> SendAsync<T>(HttpRequestMessage request)
        {
            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❗ Erreur API {request.Method} {request.RequestUri} : {response.StatusCode} - {errorContent}");
                    return default;
                }
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Exception {request.Method} {request.RequestUri} : {ex.Message}");
                return default;
            }
        }

        private async Task<bool> SendAsync(HttpRequestMessage request)
        {
            try
            {
                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❗ Erreur API {request.Method} {request.RequestUri} : {response.StatusCode} - {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❗ Exception {request.Method} {request.RequestUri} : {ex.Message}");
                return false;
            }
        }

        // 📡 Fonctions d'accès à l'API

        public async Task<UserDto> GetUserById(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;
            var req = await CreateRequest(HttpMethod.Get, $"{Apis.Users.GetById}/{userId}");
            return await SendAsync<UserDto>(req);
        }

        public async Task<bool> UpdateUserLocationAsync(string userId, double latitude, double longitude)
        {
            var req = await CreateRequest(HttpMethod.Put, $"{Apis.Users.UpdateLocation}/{userId}/location", new { Latitude = latitude, Longitude = longitude });
            return await SendAsync(req);
        }

        public async Task<List<MissionDto>> GetMissions()
        {
            var req = await CreateRequest(HttpMethod.Get, Apis.Missions.List);
            return await SendAsync<List<MissionDto>>(req) ?? new();
        }

        public async Task<MissionDto> GetMissionById(int missionId)
        {
            var req = await CreateRequest(HttpMethod.Get, $"/api/mission/{missionId}");
            return await SendAsync<MissionDto>(req) ?? new MissionDto();
        }

        public async Task<List<MissionDto>> GetMissionsForUserAsync(string userId)
        {
            var req = await CreateRequest(HttpMethod.Get, $"{Apis.Missions.GetByUser}/{userId}");
            return await SendAsync<List<MissionDto>>(req) ?? new();
        }

        public async Task<bool> AccepterMission(int missionId)
        {
            var req = await CreateRequest(HttpMethod.Post, string.Format(Apis.Missions.Accepter, missionId));
            return await SendAsync(req);
        }

        public async Task<bool> RefuserMission(int missionId)
        {
            var req = await CreateRequest(HttpMethod.Post, string.Format(Apis.Missions.Refuser, missionId));
            return await SendAsync(req);
        }

        public async Task<bool> CommencerMission(int missionId)
        {
            var req = await CreateRequest(HttpMethod.Post, string.Format(Apis.Missions.Commencer, missionId));
            return await SendAsync(req);
        }

        public async Task<bool> CloturerMission(int missionId)
        {
            var req = await CreateRequest(HttpMethod.Post, string.Format(Apis.Missions.Cloturer, missionId));
            return await SendAsync(req);
        }
    }
}