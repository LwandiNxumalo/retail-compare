using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using RetailCompare.App.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels;

public class ProductListViewModel : INotifyPropertyChanged
{
    private readonly ApiService _apiService;
    private bool _isBusy;
    private string _searchText = string.Empty;

    public ObservableCollection<ProductDto> Products { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(); }
    }

    public ICommand LoadProductsCommand { get; }
    public ICommand SearchCommand { get; }
    public ICommand AddToWatchlistCommand { get; }

    public ProductListViewModel()
    {
        _apiService = new ApiService();

        LoadProductsCommand = new Command(async () => await LoadProductsAsync());
        SearchCommand = new Command(async () => await LoadProductsAsync(SearchText));
        AddToWatchlistCommand = new Command<ProductDto>(async (product) => await AddToWatchlistAsync(product));
    }

    public async Task LoadProductsAsync(string? search = null)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            var items = await _apiService.GetProductsAsync(search);

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

    private async Task AddToWatchlistAsync(ProductDto? product)
    {
        if (product == null) return;

        string defaultUserId = "user123";

        bool success = await _apiService.AddToWatchlistAsync(defaultUserId, product.Id, product.CurrentLowestPrice);

        var mainPage = Application.Current?.Windows.FirstOrDefault()?.Page;

        if (mainPage != null)
        {
            if (success)
            {
                await mainPage.DisplayAlertAsync("Success", $"{product.Name} added to your watchlist!", "OK");
            }
            else
            {
                await mainPage.DisplayAlertAsync("Error", "Could not add product to watchlist.", "OK");
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}