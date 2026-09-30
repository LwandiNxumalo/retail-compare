using System.Net.Http.Headers;
using System.Net.Http.Json;
using RetailCompare.Shared.models;

namespace RetailCompare.App.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private const string AuthTokenKey = "auth_token";

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    #region Authentication Operations

    /// <summary>
    /// Authenticates user and securely stores JWT token.
    /// </summary>
    public async Task<bool> LoginAsync(UserLoginDto loginDto)
    {
        try
        {
            loginDto.Email = loginDto.Email.Trim().ToLower();

            // Hits https://localhost:5189/api/auth/login correctly
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    await SecureStorage.SetAsync("auth_token", result.Token);
                    return true;
                }
            }

            var error = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"[Login Failed] Status: {response.StatusCode}, Details: {error}");
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Login Exception] {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Registers a new user account.
    /// </summary>
    public async Task<bool> RegisterAsync(UserRegisterDto registerDto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    await SecureStorage.SetAsync("auth_token", result.Token);
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ApiService Register Error] {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Checks if a valid auth token is saved in SecureStorage.
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(AuthTokenKey);
            return !string.IsNullOrWhiteSpace(token);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Attaches stored JWT token to the HttpClient authorization header.
    /// </summary>
    public async Task SetAuthHeaderAsync()
    {
        try
        {
            var token = await SecureStorage.Default.GetAsync(AuthTokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[SetAuthHeaderAsync Error] {ex.Message}");
        }
    }

    /// <summary>
    /// Logs out user by clearing stored auth token and resetting headers.
    /// </summary>
    public void Logout()
    {
        SecureStorage.Default.Remove(AuthTokenKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    #endregion

    #region Product Catalog Operations

    /// <summary>
    /// Retrieves all products or filters by search query.
    /// </summary>
    public async Task<List<ProductDto>> GetProductsAsync(string? query = null)
    {
        try
        {
            await SetAuthHeaderAsync();
            var endpoint = string.IsNullOrWhiteSpace(query)
                ? "products"
                : $"products?search={Uri.EscapeDataString(query)}";

            var products = await _httpClient.GetFromJsonAsync<List<ProductDto>>(endpoint);
            return products ?? new List<ProductDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetProductsAsync Error] {ex.Message}");
            return new List<ProductDto>();
        }
    }

    /// <summary>
    /// Fetches details for a single product by ID.
    /// </summary>
    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        try
        {
            await SetAuthHeaderAsync();
            return await _httpClient.GetFromJsonAsync<ProductDto>($"products/{id}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetProductByIdAsync Error] {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Retrieves price history points for a given product.
    /// </summary>
    public async Task<List<PriceHistoryDto>> GetPriceHistoryAsync(int productId)
    {
        try
        {
            await SetAuthHeaderAsync();
            var history = await _httpClient.GetFromJsonAsync<List<PriceHistoryDto>>($"products/{productId}/price-history");
            return history ?? new List<PriceHistoryDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetPriceHistoryAsync Error] {ex.Message}");
            return new List<PriceHistoryDto>();
        }
    }

    #endregion

    #region Watchlist Operations

    /// <summary>
    /// Retrieves all items on the authenticated user's watchlist.
    /// </summary>
    public async Task<List<WatchlistItemDto>> GetWatchlistAsync()
    {
        try
        {
            await SetAuthHeaderAsync();
            var items = await _httpClient.GetFromJsonAsync<List<WatchlistItemDto>>("watchlist");
            return items ?? new List<WatchlistItemDto>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GetWatchlistAsync Error] {ex.Message}");
            return new List<WatchlistItemDto>();
        }
    }

    /// <summary>
    /// Adds a product to the user's watchlist with a target price.
    /// </summary>
    public async Task<bool> AddToWatchlistAsync(WatchlistRequestDto request)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("watchlist", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[AddToWatchlistAsync Error] {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Removes an item from the watchlist by ID.
    /// </summary>
    public async Task<bool> RemoveFromWatchlistAsync(int watchlistId)
    {
        try
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync($"watchlist/{watchlistId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[RemoveFromWatchlistAsync Error] {ex.Message}");
            return false;
        }
    }

    #endregion
}