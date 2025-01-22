using System.Net.Http.Json;
using System.Text.Json;
using TechDispoB.Models;
using TechDispoB.Services.Interfaces;

namespace TechDispoB.Services.Implementations
{
    public class AppService 
    {
        private readonly IAppService _apiService;

        public AppService(IAppService apiService)
        {
            _apiService = apiService;
        }
        public async Task<LoginResponseDto?> Login(LoginModel loginModel)
        {
            try
            {
                Console.WriteLine($"🔑 Tentative de connexion pour : {loginModel.Email}");

                var response = await _apiService.Login(loginModel);

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    Console.WriteLine("✅ Jeton JWT reçu !");
                    await SecureStorage.SetAsync("auth_token", response.Token);
                    return response;
                }
                else
                {
                    Console.WriteLine("❌ Identifiants incorrects.");
                    return null;
                }
            }
            catch (Refit.ApiException apiEx)
            {
                Console.WriteLine($"❌ Erreur API : {apiEx.StatusCode}");

                if (apiEx.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine($"⚠️ 401 Unauthorized - Vérifie ton email/mot de passe.");
                }

                Console.WriteLine($"📜 Contenu de la réponse : {await apiEx.GetContentAsAsync<string>()}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠️ Erreur générale : {ex.Message}");
                return null;
            }
        }



        //public async Task<List<MissionDto>> GetMissions()
        //{
        //    try
        //    {
        //        Console.WriteLine("📡 Récupération des missions...");
        //        return await _httpClient.GetFromJsonAsync<List<MissionDto>>(Apis.ListMissions) ?? new List<MissionDto>();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"❌ Erreur lors du chargement des missions : {ex.Message}");
        //        return new List<MissionDto>();
        //    }
        //}

        //public async Task<MissionDto> GetMissionById(int missionId)
        //{
        //    try
        //    {
        //        Console.WriteLine($"📡 Récupération de la mission ID {missionId}...");
        //        return await _httpClient.GetFromJsonAsync<MissionDto>($"api/mission/mission/{missionId}") ?? new MissionDto();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"❌ Erreur lors de la récupération de la mission {missionId} : {ex.Message}");
        //        return new MissionDto();
        //    }
        //}

        public async Task<DatabaseConnectionResponse> CanConnectToDatabase()
        {
            try
            {
                Console.WriteLine("📡 Vérification de la connexion à la base de données...");

                var response = await _apiService.CanConnectToDatabase();

                Console.WriteLine($"🔹 Réponse de l'API : {response?.Message}");

                return response ?? new DatabaseConnectionResponse { Message = "❌ Réponse API null" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Impossible de se connecter à la base de données : {ex.Message}");
                return new DatabaseConnectionResponse { Message = $"⚠️ Erreur: {ex.Message}" };
            }
        }
    }
}
