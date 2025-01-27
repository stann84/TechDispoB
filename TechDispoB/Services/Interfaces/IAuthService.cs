
namespace TechDispoB.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> Login(string email, string password);
        Task Logout();
        Task<bool> IsAuthenticated();
    }
}
