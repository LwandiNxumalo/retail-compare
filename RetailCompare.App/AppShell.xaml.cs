using RetailCompare.App.Services;
using RetailCompare.App.Views;

namespace RetailCompare.App;

public partial class AppShell : Shell
{
    private readonly ApiService _apiService;

    public AppShell(ApiService apiService)
    {
        InitializeComponent();
        _apiService = apiService;

        // Register detail & modal routes
        Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
        Routing.RegisterRoute(nameof(ProductDetailPage), typeof(ProductDetailPage));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckAuthenticationStateAsync();
    }

    private async Task CheckAuthenticationStateAsync()
    {
        try
        {
            var isAuthenticated = await _apiService.IsAuthenticatedAsync();

            if (isAuthenticated)
            {
                // Pre-load authorization header for subsequent API calls
                await _apiService.SetAuthHeaderAsync();

                // Bypass login page and navigate directly to main product catalog
                await Shell.Current.GoToAsync("//MainPage");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AppShell Auth Check Error] {ex.Message}");
        }
    }
}