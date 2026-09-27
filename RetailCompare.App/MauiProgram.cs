using Microsoft.Extensions.Logging;
using RetailCompare.App.Services;
using RetailCompare.App.ViewModels;
using RetailCompare.App.Views;

namespace RetailCompare.App
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
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register HttpClient & ApiService
            builder.Services.AddSingleton(sp => new HttpClient
            {
                BaseAddress = new Uri("https://your-api-domain.com/api/") // Replace with API URL
            });
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

            // Register Shell
            builder.Services.AddSingleton<AppShell>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}