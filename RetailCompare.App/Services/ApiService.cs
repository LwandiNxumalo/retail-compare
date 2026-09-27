using System.Net.Http.Headers;
using System.Net.Http.Json;
using RetailCompare.Shared.models;

namespace RetailCompare.App.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string TokenKey = "auth_token";
        private const string UserEmailKey = "user_email";
        private const string UserIdKey = "user_id";

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Authentication & Token Management

        public async Task SetAuthHeaderAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await SecureStorage.Default.GetAsync(TokenKey);
            return !string.IsNullOrEmpty(token);
        }

        public async Task<AuthResponseDto?> RegisterAsync(UserRegisterDto registerDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/register", registerDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SaveAuthSessionAsync(result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error (Register): {ex.Message}");
            }

            return null;
        }

        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("auth/login", loginDto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        await SaveAuthSessionAsync(result);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error (Login): {ex.Message}");
            }

            return null;
        }

        public async Task LogoutAsync()
        {
            SecureStorage.Default.Remove(TokenKey);
            SecureStorage.Default.Remove(UserEmailKey);
            SecureStorage.Default.Remove(UserIdKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            await Task.CompletedTask;
        }

        private async Task SaveAuthSessionAsync(AuthResponseDto auth)
        {
            await SecureStorage.Default.SetAsync(TokenKey, auth.Token);
            await SecureStorage.Default.SetAsync(UserEmailKey, auth.Email);
            await SecureStorage.Default.SetAsync(UserIdKey, auth.UserId);
            await SetAuthHeaderAsync();
        }

        public async Task<string?> GetCurrentUserIdAsync()
        {
            return await SecureStorage.Default.GetAsync(UserIdKey);
        }

        #endregion

        #region Product Endpoints

        public async Task<List<ProductDto>> GetProductsAsync(string? search = null)
        {
            try
            {
                await SetAuthHeaderAsync();
                var url = string.IsNullOrWhiteSpace(search)
                    ? "products"
                    : $"products?search={Uri.EscapeDataString(search)}";

                var response = await _httpClient.GetFromJsonAsync<List<ProductDto>>(url);
                return response ?? new List<ProductDto>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error (GetProducts): {ex.Message}");
                return new List<ProductDto>();
            }
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                await SetAuthHeaderAsync();
                return await _httpClient.GetFromJsonAsync<ProductDto>($"products/{id}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"API Error (GetProductById): {ex.Message}");
                return null;
            }
        }

        #endregion

        #region Watchlist Endpoints

        public async Task<bool> AddToWatchlistAsync(string userId, int productId, decimal targetPrice)
        {
            try
            {
                await SetAuthHeaderAsync();
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
                System.Diagnostics.Debug.WriteLine($"API Error (AddToWatchlist): {ex.Message}");
                return false;
            }
        }

        #endregion
    }
}