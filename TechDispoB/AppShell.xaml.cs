using TechDispoB.Components.Pages;

namespace TechDispoB
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //CheckAuthentication();
            // 📌 Enregistrer la route de MissionPage pour permettre la navigation
            //Routing.RegisterRoute("mission-page", typeof(MissionPage));
            //Routing.RegisterRoute("login-page", typeof(LoginPage));
            Routing.RegisterRoute("missions", typeof(MissionsListPage));
        }

        private async void CheckAuthentication()
        {
            var token = await SecureStorage.GetAsync("token");

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("🔒 Aucun utilisateur connecté. Redirection vers Login.");
                await Shell.Current.GoToAsync("//login");
            }
        }

        //private async void OnLogoutClicked(object sender, EventArgs e)
        //{
        //    await SecureStorage.SetAsync("token", ""); // Supprime le token
        //    IsAuthenticated = false;
        //    IsNotAuthenticated = true;
        //    Console.WriteLine("🔓 Déconnexion réussie.");
        //    await Current.GoToAsync("//login"); // Redirige vers la connexion
        //}


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

