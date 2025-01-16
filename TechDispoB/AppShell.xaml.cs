using TechDispoB.Components.Pages;

namespace TechDispoB
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Enregistrement des routes supplémentaires
            Routing.RegisterRoute("mission-details", typeof(MissionPage));
        }
    }
}
