using Newtonsoft.Json.Linq;
using TechDispoB.Models;

namespace TechDispoB.Services
{
    public interface IAppService 
    {

        public Task<bool> UpdateUserLocationAsync(string userId, double latitude, double longitude);

        event Action? OnAuthStateChanged; // ✅ Événement pour notifier Blazor du changement d'état

        // Missions 
        public Task<List<MissionDto>> GetMissions();
        public Task<MissionDto> GetMissionById(int missionId);
        public Task<List<MissionDto>> GetMissionsForUserAsync(string userId);
        public Task<UserDto> GetUserById(string userId);
        public Task<bool> AccepterMission(int missionId);
        public Task<bool> RefuserMission(int missionId);
        public Task<bool> CommencerMission(int missionId);
        public Task<bool> CloturerMission(int missionId);

    }
}
