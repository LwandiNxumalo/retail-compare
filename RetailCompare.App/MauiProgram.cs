using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;
using RetailCompare.App.Services;
using RetailCompare.App.ViewModels;
using RetailCompare.App.Views;

namespace RetailCompare.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Determine the correct host depending on platform
#if ANDROID
        string baseUrl = "https://10.0.2.2:5189/";
#else
        string baseUrl = "https://localhost:5189/";
#endif

        // Register HttpClient with SSL bypass for DEBUG mode
        builder.Services.AddSingleton(sp =>
        {
#if DEBUG
            var handler = GetInsecureHandler();
            var client = new HttpClient(handler);
#else
            var client = new HttpClient();
#endif
            client.BaseAddress = new Uri(baseUrl);
            return client;
        });

        // Register ApiService
        builder.Services.AddSingleton<ApiService>();

        // Register ViewModels
        builder.Services.AddTransient<ProductListViewModel>();
        builder.Services.AddTransient<WatchlistViewModel>();
        builder.Services.AddTransient<ProductDetailViewModel>();
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();

        // Register Views
        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<WatchlistPage>();
        builder.Services.AddTransient<ProductDetailPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();

        // Register App Shell & Application
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddSingleton<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    public static HttpClientHandler GetInsecureHandler()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        return handler;
    }
}