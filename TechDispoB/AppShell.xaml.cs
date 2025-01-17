namespace TechDispoB
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            CheckAuthentication();
        }

        private async void CheckAuthentication()
        {
            var token = await SecureStorage.GetAsync("token");
            bool isAuthenticated = !string.IsNullOrEmpty(token);

            // Activer ou désactiver le menu Déconnexion selon l'authentification
            // Active/Désactive le bouton Déconnexion
            if (LogoutMenuItem != null)
            {
                LogoutMenuItem.IsEnabled = isAuthenticated;
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await SecureStorage.SetAsync("token", ""); // Supprime le token
            IsAuthenticated = false;
            IsNotAuthenticated = true;
            Console.WriteLine("🔓 Déconnexion réussie.");
            Current.GoToAsync("//login"); // Redirige vers la connexion
        }


        public bool IsAuthenticated { get; set; }
        public bool IsNotAuthenticated { get; set; }
    }

    // UnComment the below method to handle Shell Menu item click event
    // And ensure appropriate page definitions are available for it work as expected
    /*
    private async void OnMenuItemClicked(object sender, EventArgs e)
    {
        await Current.GoToAsync("//login");
    }
    */
}

