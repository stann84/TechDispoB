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
        public AppService()
        {
            _httpClient = HttpClientService.CreateHttpClient();
        }
        // User 
        public event Action? OnAuthStateChanged;
        public async Task<bool> CanConnectToDatabase()
        {
            try
            {
                var response = await _httpClient.GetAsync(Apis.CheckDatabaseConnection);

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
                var response = await _httpClient.PostAsJsonAsync(Apis.Login, loginModel);
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

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token) && loginResponse.User != null)
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
        public async Task<bool> SendFCMTokenAsync(string fcmToken, string jwtToken)
        {
            try
            {
                // Vérifier si le token JWT est valide
                if (string.IsNullOrEmpty(jwtToken))
                {
                    Console.WriteLine("❌ Aucun jeton JWT trouvé, annulation de l'envoi du FCM Token.");
                    return false;
                }

                if (string.IsNullOrEmpty(fcmToken))
                {
                    Console.WriteLine("❌ Aucun FCM Token disponible, annulation.");
                    return false;
                }

                var data = new UserDto
                {
                   // Id = "", // On ne met rien car l'API récupère `userId` via JWT
                    FCMToken = fcmToken
                };

                Console.WriteLine($"🔹 JSON envoyé : {JsonSerializer.Serialize(data)}"); // Vérification

                // 🔹 Envoi de la requête avec `PostAsJsonAsync`
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

                var response = await _httpClient.PostAsJsonAsync(Apis.UpdateFCMToken, data);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Erreur lors de l'envoi du token FCM : {response.StatusCode} - {errorContent}");
                    return false;
                }

                Console.WriteLine("✅ FCM Token mis à jour avec succès !");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Erreur lors de l'envoi du token FCM : {ex.Message}");
                return false;
            }
        }

        public async Task Logout()
        {
            await SecureStorage.SetAsync("auth_token", ""); // Efface le token
            Console.WriteLine("Utilisateur déconnecté !");
            OnAuthStateChanged?.Invoke(); // Notifie Blazor

        }
        public async Task<bool> UpdateUserLocationAsync(string userId, double latitude, double longitude)
        {
            var url = $"{Apis.UpdateUserLocation}/{userId}/location";

            var data = new
            {
                Latitude = latitude,
                Longitude = longitude
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erreur lors de la mise à jour de la localisation : {response.StatusCode} - {errorContent}");
            }

            return response.IsSuccessStatusCode;
        }
        public async Task<UserDto> GetUserById(string userId)
        {
            return await _httpClient.GetFromJsonAsync<UserDto>($"{Apis.GetUserById}/{userId}") ?? new UserDto();
        }

        // Missions
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
            return await _httpClient.GetFromJsonAsync<List<MissionDto>>($"{Apis.GetMissionsForUser}/{userId}")?? new List<MissionDto>();
        }

        public async Task<bool> AccepterMission(int missionId)
        {
            var url = string.Format(Apis.AcceptMission, missionId);
            var response = await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erreur API : {error}");
            }

            return true;
        }

        public async Task<bool> RefuserMission(int missionId)
        {
            var url = string.Format(Apis.RefuseMission, missionId);
            var response = await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erreur API : {error}");
            }

            return true;
        }

        public async Task<bool> CommencerMission(int missionId)
        {
            var url = string.Format(Apis.CommencerMission, missionId);
            var response = await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erreur API : {error}");
            }

            return true;
        }

        public async Task<bool> CloturerMission(int missionId)
        {
            var url = string.Format(Apis.CloturerMission, missionId);
            var response = await _httpClient.PostAsync(url, null);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erreur API : {error}");
            }

            return true;
        }


    }
}
