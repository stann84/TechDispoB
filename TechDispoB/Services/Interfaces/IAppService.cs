using Newtonsoft.Json.Linq;
using TechDispoB.Models;

namespace TechDispoB.Services
{
    public interface IAppService 
    {
        //User
        public Task<LoginResponseDto?> Login(LoginDto loginModel);
        public Task<bool> CanConnectToDatabase();
        public Task<bool> IsAuthenticated();
        public Task<bool> SendFCMTokenAsync(string fcmToken, string jwtToken);
        public Task Logout();
        public Task<bool> UpdateUserLocationAsync(string userId, double latitude, double longitude);

        event Action? OnAuthStateChanged; // ✅ Événement pour notifier Blazor du changement d'état

        // Missions 
        public Task<List<MissionDto>> GetMissions();
        public Task<MissionDto> GetMissionById(int missionId);
        public Task<List<MissionDto>> GetMissionsForUserAsync(string userId);

        public Task<UserDto> GetUserById(string userId);
        public Task<bool> AccepterMission(int missionId);
        public Task<bool> RefuserMission(int missionId);

    }
}
