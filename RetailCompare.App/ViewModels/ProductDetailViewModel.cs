using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailCompare.App.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels
{
    [QueryProperty(nameof(ProductId), "id")]
    public partial class ProductDetailViewModel : ObservableObject
    {
        private readonly ApiService _apiService;
        private const string CurrentUserId = "user123";

        [ObservableProperty]
        private int _productId;

        [ObservableProperty]
        private ProductDto? _product;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isAddingToWatchlist;

        public ProductDetailViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        partial void OnProductIdChanged(int value)
        {
            if (value > 0)
            {
                _ = LoadProductDetailsAsync(value);
            }
        }

        [RelayCommand]
        public async Task LoadProductDetailsAsync(int id)
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                Product = await _apiService.GetProductByIdAsync(id);

                if (Product == null && Shell.Current != null)
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Product details could not be found.", "OK");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ProductDetailViewModel Error] {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task AddToWatchlistAsync()
        {
            if (Product == null || Shell.Current == null) return;

            // Prompt user for a target price, defaulting to current price
            string result = await Shell.Current.DisplayPromptAsync(
            "Add to Watchlist",
            $"Enter your target price for {Product.Name}:",
            accept: "Add",
            cancel: "Cancel",
            placeholder: $"{Product.CurrentLowestPrice:F2}",
            keyboard: Keyboard.Numeric);

            if (string.IsNullOrWhiteSpace(result)) return;

            if (decimal.TryParse(result, out decimal targetPrice) && targetPrice > 0)
            {
                IsAddingToWatchlist = true;
                bool success = await _apiService.AddToWatchlistAsync(CurrentUserId, Product.Id, targetPrice);
                IsAddingToWatchlist = false;

                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Success", $"{Product.Name} added to your watchlist at R{targetPrice:F2}!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Failed to add item to watchlist. Please try again.", "OK");
                }
            }
            else
            {
                await Shell.Current.DisplayAlertAsync("Invalid Price", "Please enter a valid numeric target price.", "OK");
            }
        }
    }
}