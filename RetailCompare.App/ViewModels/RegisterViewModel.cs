using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RetailCompare.App.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        public partial string Username { get; set; }

        [ObservableProperty]
        public partial string Email { get; set; }

        [ObservableProperty]
        public partial string Password { get; set; }

        [ObservableProperty]
        public partial bool SaveCredentials { get; set; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RegisterCommand))]
        public partial bool IsBusy { get; set; }

        [ObservableProperty]
        public partial string ErrorMessage { get; set; }

        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService;

            // Initialize property defaults in constructor
            Username = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            SaveCredentials = true;
            IsBusy = false;
            ErrorMessage = string.Empty;
        }

        private bool CanRegister() => !IsBusy;

        [RelayCommand(CanExecute = nameof(CanRegister))]
        public async Task RegisterAsync()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please fill in all required fields.";
                return;
            }

            if (!IsPasswordStrong(Password))
            {
                ErrorMessage = "Password must be at least 8 characters long and contain an uppercase letter, a number, and a special character.";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var registerDto = new UserRegisterDto
                {
                    FullName = Username.Trim(),
                    Email = Email.Trim(),
                    Password = Password
                };

                // RegisterAsync returns a non-nullable bool, so check it directly
                bool success = await _apiService.RegisterAsync(registerDto);

                if (success)
                {
                    if (SaveCredentials)
                    {
                        Preferences.Set("SavedEmail", Email.Trim());
                    }

                    // Use async DisplayAlertAsync for .NET 10 MAUI
                    await Shell.Current.DisplayAlertAsync("Success", "Account created successfully! Please sign in.", "OK");
                    await Shell.Current.GoToAsync("//LoginPage");
                }
                else
                {
                    ErrorMessage = "Registration failed. This email might already be registered.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during registration. Please check your network connection.";
                System.Diagnostics.Debug.WriteLine($"[RegisterViewModel Error] {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool IsPasswordStrong(string password)
        {
            if (password.Length < 8) return false;
            if (!Regex.IsMatch(password, @"[A-Z]")) return false;
            if (!Regex.IsMatch(password, @"[0-9]")) return false;
            if (!Regex.IsMatch(password, @"[\W_]")) return false;
            return true;
        }
    }
}