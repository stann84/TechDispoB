using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TechDispoB.Models;
using TechDispoB.Services.Interfaces;

namespace TechDispoB.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public event Action? OnAuthStateChanged;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            Console.WriteLine($"✅ HttpClient base address = {_httpClient.BaseAddress}");
        }

        // 🔐 Vérifie si un token est stocké localement
        public async Task<bool> IsAuthenticated()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }

        // 🔐 Connexion de l'utilisateur : envoie les identifiants à l'API et stocke le token
        public async Task<LoginResponseDto?> Login(LoginDto loginModel)
        {
            try
            {
                Console.WriteLine($"🔐 Envoi LoginDto : {JsonSerializer.Serialize(loginModel)}");
                var response = await _httpClient.PostAsJsonAsync(Apis.Users.Login, loginModel);
                var raw = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"📡 Réponse brute API : {raw}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"❌ Échec HTTP : {response.StatusCode}");
                    return null;
                }

                var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(raw, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    await SecureStorage.SetAsync("auth_token", loginResponse.Token);
                    await SecureStorage.SetAsync("userId", loginResponse.User.Id);
                    Console.WriteLine("✅ JWT stored.");
                    OnAuthStateChanged?.Invoke();
                }

                return loginResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login exception : {ex.Message}");
                return null;
            }
        }

        // 🔓 Déconnexion de l'utilisateur
        public async Task Logout()
        {
            await SecureStorage.SetAsync("auth_token", "");
            OnAuthStateChanged?.Invoke();
            Console.WriteLine("Utilisateur déconnecté.");
        }

        // 📡 Vérifie la connexion à la base de données côté API
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

        // 🔥 Envoie du token Firebase Cloud Messaging à l'API
        public async Task<bool> SendFCMTokenAsync(string fcmToken, string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(fcmToken))
                    return false;

                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", jwtToken);

                var data = new UserDto { FCMToken = fcmToken };
                var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(Apis.Users.UpdateFCMToken, content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur FCM : {ex.Message}");
                return false;
            }
        }

        // 🔑 Changement de mot de passe sécurisé
        public async Task<bool> ChangerMotDePasse(ChangerMotDePasseDto dto)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token)) return false;

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync(Apis.Users.ChangerMotDePasse, dto);
            return response.IsSuccessStatusCode;
        }
    }
}