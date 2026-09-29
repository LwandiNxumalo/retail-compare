using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailCompare.App.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels;

public partial class WatchlistViewModel : ObservableObject
{
    private readonly ApiService _apiService;
    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial bool IsEmpty { get; set; }

    public ObservableCollection<WatchlistItemDto> WatchlistItems { get; } = new();

    public WatchlistViewModel(ApiService apiService)
    {
        _apiService = apiService;
        IsLoading = false;
        IsEmpty = false;
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

            // Fetch current user's watchlist using stored JWT token
            var items = await _apiService.GetWatchlistAsync();

            if (items == null || items.Count == 0)
            {
                IsEmpty = true;
                return;
            }

            foreach (var item in items)
            {
                WatchlistItems.Add(item);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[WatchlistViewModel Error] {ex.Message}");
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
    public async Task RemoveItemAsync(WatchlistItemDto item)
    {
        if (item == null || Shell.Current == null) return;

        bool confirm = await Shell.Current.DisplayAlertAsync("Remove", $"Remove {item.ProductName} from your watchlist?", "Yes", "No");
        if (!confirm) return;

        bool success = await _apiService.RemoveFromWatchlistAsync(item.Id);
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