using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RetailCompare.App.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels;

public partial class WatchlistViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    private const string CurrentUserId = "user123";

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    public ObservableCollection<WatchlistItemDisplayModel> WatchlistItems { get; } = new();

    public WatchlistViewModel(ApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    public async Task LoadWatchlistAsync()
    {
        if (IsLoading) return;

        try
        {
            IsLoading = true;
            IsEmpty = false;
            WatchlistItems.Clear();

            // 1. Fetch user's watchlist items
            var watchlist = await _apiService.GetWatchlistAsync(CurrentUserId);

            if (watchlist == null || !watchlist.Any())
            {
                IsEmpty = true;
                return;
            }

            // 2. Fetch all products to join display details (Name, Price)
            var products = await _apiService.GetProductsAsync();
            var productDict = products?.ToDictionary(p => p.Id) ?? new();

            foreach (var item in watchlist)
            {
                var productName = productDict.TryGetValue(item.ProductId, out var product)
                    ? product.Name
                    : $"Product #{item.ProductId}";

                var currentPrice = product != null ? product.CurrentLowestPrice : 0;

                WatchlistItems.Add(new WatchlistItemDisplayModel
                {
                    UserId = item.UserId,
                    ProductId = item.ProductId,
                    ProductName = productName,
                    CurrentPrice = currentPrice,
                    TargetPrice = item.TargetPrice
                });
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlertAsync("Error", $"Failed to load watchlist: {ex.Message}", "OK");
            }
        }
        finally
        {
            IsLoading = false;
            IsEmpty = WatchlistItems.Count == 0;
        }
    }

    [RelayCommand]
    public async Task RemoveItemAsync(WatchlistItemDisplayModel item)
    {
        if (item == null || Shell.Current == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync("Remove", $"Remove {item.ProductName} from your watchlist?", "Yes", "No");
        if (!confirm) return;

        bool success = await _apiService.RemoveFromWatchlistAsync(item.UserId, item.ProductId);
        if (success)
        {
            WatchlistItems.Remove(item);
            IsEmpty = WatchlistItems.Count == 0;
        }
        else
        {
            await Shell.Current.DisplayAlertAsync("Error", "Could not remove item from watchlist.", "OK");
        }
    }
}

public class WatchlistItemDisplayModel
{
    public string UserId { get; set; } = string.Empty;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal TargetPrice { get; set; }
}