using TechDispo.Models;
using TechDispoB.Models;

namespace TechDispoB.Services
{
    public interface IAppService 
    {
        public Task<string?> Login(LoginModel loginModel);
        public Task<List<Mission>> GetMissions();
        public Task<Mission> GetMissionById(int missionId);
        public Task<bool> CanConnectToDatabase();

    }
}
