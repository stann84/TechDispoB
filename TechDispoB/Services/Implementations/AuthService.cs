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

        // 🔐 Connexion de l'utilisateur
        public async Task<LoginResponseDto?> Login(LoginDto loginModel)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(Apis.Users.Login, loginModel);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed : {response.StatusCode}, {errorContent}");
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

        /// 🛡️ Déconnexion de l'utilisateur

        public async Task Logout()
        {
            await SecureStorage.SetAsync("auth_token", "");
            OnAuthStateChanged?.Invoke();
            Console.WriteLine("Utilisateur déconnecté.");
        }

        /// 🔥 Envoie du FCM Token

        public async Task<bool> SendFCMTokenAsync(string fcmToken, string jwtToken)
        {
            try
            {
                if (string.IsNullOrEmpty(jwtToken) || string.IsNullOrEmpty(fcmToken))
                    return false;

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

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

        /// <summary>
        /// Change le mot de passe de l'utilisateur actuellement authentifié.
        /// </summary>
        public async Task<bool> ChangerMotDePasse(ChangerMotDePasseDto dto)
        {
            // Récupère le token JWT stocké
            var token = await SecureStorage.GetAsync("auth_token");
            if (string.IsNullOrEmpty(token))
                return false;

            // Ajoute l'en-tête Authorization
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Appelle l'API pour changer le mot de passe
            var response = await _httpClient.PostAsJsonAsync(Apis.Users.ChangerMotDePasse, dto);
            return response.IsSuccessStatusCode;
        }
    }
}
