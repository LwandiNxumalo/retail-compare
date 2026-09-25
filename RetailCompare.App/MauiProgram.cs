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

            // Register Services
            builder.Services.AddSingleton<ApiService>();

            // Register ViewModels
            builder.Services.AddTransient<WatchlistViewModel>();
            builder.Services.AddTransient<ProductDetailViewModel>();

            // Register Views
            builder.Services.AddTransient<WatchlistPage>();
            builder.Services.AddTransient<ProductDetailPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}