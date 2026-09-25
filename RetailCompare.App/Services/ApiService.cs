using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Devices;
using RetailCompare.Shared.models;

namespace RetailCompare.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    // Platform-specific localhost base URL for Android Emulator vs Desktop
    private static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:44362/api/"
            : "http://localhost:44362/api/";

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl)
        };
    }

    public async Task<List<ProductDto>> GetProductsAsync(string? search = null)
    {
        try
        {
            var url = string.IsNullOrWhiteSpace(search)
                ? "products"
                : $"products?search={Uri.EscapeDataString(search)}";

            var response = await _httpClient.GetFromJsonAsync<List<ProductDto>>(url);
            return response ?? new List<ProductDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            return new List<ProductDto>();
        }
    }

    public async Task<List<WatchlistRequest>> GetWatchlistAsync(string userId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<WatchlistRequest>>($"watchlist/{Uri.EscapeDataString(userId)}");
            return response ?? new List<WatchlistRequest>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Watchlist API Error: {ex.Message}");
            return new List<WatchlistRequest>();
        }
    }

    public async Task<bool> AddToWatchlistAsync(string userId, int productId, decimal targetPrice)
    {
        try
        {
            var request = new WatchlistRequest
            {
                UserId = userId,
                ProductId = productId,
                TargetPrice = targetPrice
            };

            var response = await _httpClient.PostAsJsonAsync("watchlist", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Watchlist API Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RemoveFromWatchlistAsync(string userId, int productId)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"watchlist/{Uri.EscapeDataString(userId)}/{productId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Watchlist Delete Error: {ex.Message}");
            return false;
        }
    }
}