using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailCompare.App.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels
{
    public partial class ProductListViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;

        public ObservableCollection<ProductDto> Products { get; } = [];

        public ProductListViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        public async Task LoadProductsAsync(string? search = null)
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var items = await _apiService.GetProductsAsync(search ?? SearchText);

                Products.Clear();
                foreach (var item in items)
                {
                    Products.Add(item);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task AddToWatchlistAsync(ProductDto? product)
        {
            if (product == null) return;

            var request = new WatchlistRequestDto
            {
                ProductId = product.Id,
                TargetPrice = product.CurrentLowestPrice
            };

            bool success = await _apiService.AddToWatchlistAsync(request);

            if (Shell.Current != null)
            {
                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Success", $"{product.Name} added to your watchlist!", "OK");
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Error", "Could not add product to watchlist.", "OK");
                }
            }
        }
    }
}