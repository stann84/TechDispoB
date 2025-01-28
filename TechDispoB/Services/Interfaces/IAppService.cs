using TechDispoB.Models;

namespace TechDispoB.Services
{
    public interface IAppService 
    {
        public Task<LoginResponseDto?> Login(LoginDto loginModel);
        public Task<List<MissionDto>> GetMissions();
        public Task<MissionDto> GetMissionById(int missionId);
        public Task<bool> CanConnectToDatabase();
        public Task<bool> IsAuthenticated();
        public Task<List<MissionDto>> GetMissionsForUserAsync(string userId);
        public Task Logout();
        event Action? OnAuthStateChanged; // ✅ Événement pour notifier Blazor du changement d'état


    }
}
