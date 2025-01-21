using TechDispoMobile.Components.Pages;

namespace TechDispoMobile
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("weather", typeof(Weather));
        }



        // UnComment the below method to handle Shell Menu item click event
        // And ensure appropriate page definitions are available for it work as expected

        //private async void OnMenuItemClicked(object sender, EventArgs e)
        //{
        //    await Current.GoToAsync("//login");
        //}



    }
}
