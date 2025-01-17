using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechDispoB.Services
{
    public class AuthState
    {
        public bool IsAuthenticated { get; private set; }

        public async Task CheckAuthenticationAsync()
        {
            var token = await SecureStorage.GetAsync("token");
            IsAuthenticated = !string.IsNullOrEmpty(token);
        }

        public async Task Logout()
        {
            await SecureStorage.SetAsync("token", "");
            IsAuthenticated = false;
        }
    }
}
