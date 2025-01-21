using Microsoft.Extensions.Logging;
using Refit;
using TechDispoB.Services;
using TechDispoB.Services.Implementations;
using TechDispoB.Services.Interfaces;

namespace TechDispoB
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            Console.WriteLine("🚀 MAUI App Initialization Started");

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // 🔹 Configurer Refit avec HttpClient et le handler qui ajoute le token
            builder.Services.AddRefitClient<IAppService>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("https://a4d3-2a01-e0a-1d4-b530-4885-aba6-e01a-927c.ngrok-free.app"); 
                })
                .AddHttpMessageHandler<AuthHeaderHandler>(); // 🔹 Ajout du Handler pour le token JWT

            // 🔹 Ajouter Blazor WebView pour l'UI
            builder.Services.AddMauiBlazorWebView();

            // 🔹 Enregistrer `AuthHeaderHandler` pour ajouter automatiquement le token
            builder.Services.AddTransient<AuthHeaderHandler>();

            // 🔹 Enregistrer AppService qui utilisera HttpClient
            builder.Services.AddSingleton<IAppService, AppService>();

            // 🔹 Enregistrer AuthState et AppShell
            builder.Services.AddSingleton<AuthState>();
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            Console.WriteLine("✅ MAUI App Initialization Complete");

            return builder.Build();
        }
    }
}
