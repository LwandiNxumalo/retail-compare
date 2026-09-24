using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Json;
using RetailCompare.Shared.models;

namespace RetailCompare.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    // Platform-specific localhost base URL for Android Emulator vs Desktop
    private static string BaseUrl =>
        DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5189/api/" // 10.0.2.2 maps to host machine localhost in Android Emulator
            : "http://localhost:5189/api/";

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
}
