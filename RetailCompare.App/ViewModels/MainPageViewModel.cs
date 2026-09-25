using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailCompare.App.Services;
using RetailCompare.App.Views;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private ObservableCollection<ProductDto> _products = new();

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isEmpty;

        public MainPageViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        partial void OnSearchTextChanged(string value)
        {
            // Trigger auto-search when text is cleared or typed
            _ = SearchProductsAsync();
        }

        [RelayCommand]
        public async Task LoadProductsAsync()
        {
            await SearchProductsAsync();
        }

        [RelayCommand]
        public async Task SearchProductsAsync()
        {
            if (IsLoading) return;

            try
            {
                IsLoading = true;
                IsEmpty = false;

                var items = await _apiService.GetProductsAsync(SearchText);

                Products.Clear();
                foreach (var item in items)
                {
                    Products.Add(item);
                }

                IsEmpty = Products.Count == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MainPageViewModel Error] {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task SelectProductAsync(ProductDto product)
        {
            if (product == null) return;

            // Navigate using Shell route with ID parameter
            await Shell.Current.GoToAsync($"{nameof(ProductDetailPage)}?id={product.Id}");
        }
    }
}