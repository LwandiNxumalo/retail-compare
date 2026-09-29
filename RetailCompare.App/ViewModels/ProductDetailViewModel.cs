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

        [ObservableProperty]
        public partial int ProductId { get; set; }

        [ObservableProperty]
        public partial ProductDto? Product { get; set; }

        [ObservableProperty]
        public partial bool IsLoading { get; set; }

        [ObservableProperty]
        public partial bool IsAddingToWatchlist { get; set; }

        public ProductDetailViewModel(ApiService apiService)
        {
            _apiService = apiService;
            IsLoading = false;
            IsAddingToWatchlist = false;
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
                bool success = await _apiService.AddToWatchlistAsync(Product.Id, targetPrice);
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