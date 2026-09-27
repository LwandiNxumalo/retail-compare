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
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public LoginViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        public async Task LoginAsync()
        {
            if (IsBusy) return;

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
                    // Reset fields and navigate to Main App
                    Email = string.Empty;
                    Password = string.Empty;
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    ErrorMessage = "Invalid credentials. Please try again.";
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
        public async Task GoToRegisterAsync()
        {
            await Shell.Current.GoToAsync(nameof(Views.RegisterPage));
        }
    }
}