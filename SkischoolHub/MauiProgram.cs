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

        string baseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5001/"
            : "http://localhost:5001/";

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri(baseUrl)
        });

        builder.Services.AddSingleton<IApiService, ApiService>();

        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegistrationViewModel>();
        builder.Services.AddTransient<MainViewModel>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegistrationPage>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}