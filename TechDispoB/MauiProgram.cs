using Microsoft.Extensions.Logging;
using TechDispoB.Services;
using TechDispoB.Services.Implementations;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace TechDispoB
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Enregistrer les services
            builder.Services.AddSingleton<IAppService, AppService>();

            // Nécessaire pour Blazor dans .NET MAUI
            builder.Services.AddMauiBlazorWebView();


            builder.Logging.AddDebug();


            return builder.Build();
        }
    }
}
