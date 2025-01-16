using TechDispoB.Services;

namespace TechDispoB
{
    public partial class App : Application
    {
        public static IAppService AppServiceInstance { get; private set; }

        public App(IAppService appService)
        {
            InitializeComponent();

            // Stocker l'instance du service
            AppServiceInstance = appService;

            // Définir la page principale comme AppShell
            MainPage = new AppShell();

            // Valider l'utilisateur au démarrage
            ValidateUserOnStartup();
        }

        private async void ValidateUserOnStartup()
        {
            try
            {
                // Récupérer le token depuis SecureStorage
                var token = await SecureStorage.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(token))
                {
                    // Si un token est présent, validez-le
                    var isValid = await AppServiceInstance.ValidateToken(token);

                    if (isValid)
                    {
                        // Rediriger vers la page des missions si le token est valide
                        await Shell.Current.GoToAsync("missions-list-page");
                        return;
                    }
                }

                // Si aucun token ou token invalide, rediriger vers la page de connexion
                await Shell.Current.GoToAsync("login-page");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la récupération du token : {ex.Message}");
                await Shell.Current.GoToAsync("login-page");
            }
        }


    }
}
