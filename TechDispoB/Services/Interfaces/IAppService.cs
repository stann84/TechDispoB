using Refit;
using TechDispoB.Models;

namespace TechDispoB.Services.Interfaces
{
    public interface IAppService 
    {
        [Post("/auth/login")]
        Task<LoginResponse?> Login([Body] LoginModel loginModel);

        [Get("/api/missions")]
        Task<List<MissionDto>> GetMissions();

        [Get("/api/mission/{missionId}")]
        Task<MissionDto> GetMissionById(int missionId);

        [Get("/api/connectdatabase")]
        Task<bool> CanConnectToDatabase();

    }
}
