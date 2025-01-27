using TechDispoB.Models;

namespace TechDispoB.Services
{
    public interface IAppService 
    {
        public Task<LoginResponse?> Login(LoginModel loginModel);
        public Task<List<MissionDto>> GetMissions();
        public Task<MissionDto> GetMissionById(int missionId);
        public Task<bool> CanConnectToDatabase();
        public Task<bool> IsAuthenticated();

    }
}
