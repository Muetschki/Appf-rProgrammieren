using Microsoft.Extensions.Logging;
using SkischoolHub.Services;
using SkischoolHub.ViewModels;
using SkischoolHub.Views;

namespace SkischoolHub;

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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // Services registrieren
        builder.Services.AddSingleton<IApiService, ApiService>();

        // ViewModels registrieren
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegistrationViewModel>();
        builder.Services.AddTransient<MainViewModel>();

        // Pages registrieren
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegistrationPage>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
