
using TechDispoB.Models;

namespace TechDispoB.Services.Interfaces
{
    public interface IAuthService
    {
        event Action? OnAuthStateChanged;
        Task<LoginResponseDto?> Login(LoginDto loginModel); 
        Task Logout();
        Task<bool> IsAuthenticated();
        Task<bool> SendFCMTokenAsync(string fcmToken, string jwtToken);
        Task<bool> CanConnectToDatabase();
    }
}
