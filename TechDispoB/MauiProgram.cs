using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using TechDispoB.Services;
using TechDispoB.Services.Implementations;
using Plugin.Firebase.CloudMessaging;
using Microsoft.Maui.LifecycleEvents;
using TechDispoB.Services.Interfaces;

#if IOS
    using Plugin.Firebase.Core.Platforms.iOS;
#elif ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace TechDispoB
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .RegisterFirebaseServices()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            // injection de httpClientService pour appService et authService

            var baseUri = new Uri("https://2098-2a01-e0a-1d4-b530-ed35-8324-7d21-92ab.ngrok-free.app/api/");

            builder.Services.AddHttpClient<IAppService, AppService>(client =>
            {
                client.BaseAddress = baseUri;
            });
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = baseUri;
            });

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
//#if IOS
//                events.AddiOS(iOS => iOS.WillFinishLaunching((_, __) => {
//                    CrossFirebase.Initialize();
//                    FirebaseCloudMessagingImplementation.Initialize();
//                    return false;
//                }));
#if ANDROID
        events.AddAndroid(android => android.OnCreate((activity, _) =>
        CrossFirebase.Initialize(activity)));
#endif
            });

            return builder;
        }
    }




}
