using Microsoft.Extensions.Logging;
using TechDispoB.Services;
using TechDispoB.Services.Implementations;

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

            builder.Services.AddMauiBlazorWebView();

            // Enregistrer AppService en tant que Singleton
            builder.Services.AddSingleton<IAppService, AppService>();
            builder.Services.AddSingleton<AuthState>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
