using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailCompare.App.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        public partial string Email { get; set; } = string.Empty;

        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool RememberMe { get; set; } = true;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        public partial bool IsBusy { get; set; } = false;

        [ObservableProperty]
        public partial string ErrorMessage { get; set; } = string.Empty;

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            // Auto-fill saved email if available
            var savedEmail = Preferences.Get("SavedEmail", string.Empty);
            if (!string.IsNullOrEmpty(savedEmail))
            {
                Email = savedEmail;
                RememberMe = true;
            }
        }

        private bool CanLogin() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanLogin))]
        public async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both email and password.";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var request = new UserLoginDto { Email = Email.Trim(), Password = Password };
                var result = await _apiService.LoginAsync(request);

                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    // Handle Remember Me feature
                    if (RememberMe)
                    {
                        Preferences.Set("SavedEmail", Email.Trim());
                    }
                    else
                    {
                        Preferences.Remove("SavedEmail");
                    }

                    Password = string.Empty;
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    ErrorMessage = "Invalid credentials. Please check your email and password.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Login failed. Please check your network connection.";
                System.Diagnostics.Debug.WriteLine($"[LoginViewModel Error] {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task ForgotPasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter your email address to reset your password.";
                return;
            }

            // Prompt user or execute password reset API call
            await Shell.Current.DisplayAlertAsync("Password Reset", $"A password reset link has been sent to {Email}.", "OK");
        }

        [RelayCommand]
        public async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
        }
    }
}