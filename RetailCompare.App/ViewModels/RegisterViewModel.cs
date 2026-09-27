using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IntelliJ.Lang.Annotations;
using RetailCompare.App.Services;
using RetailCompare.Shared.models;

namespace RetailCompare.App.ViewModels
{
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string _email = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string _confirmPassword = string.Empty;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        public RegisterViewModel(ApiService apiService)
        {
            _apiService = apiService;
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            if (IsBusy) return;

            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please complete all fields.";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var request = new UserRegisterDto
                {
                    FullName = FullName.Trim(),
                    Email = Email.Trim(),
                    Password = Password
                };

                var result = await _apiService.RegisterAsync(request);

                if (result != null && !string.IsNullOrEmpty(result.Token))
                {
                    await Shell.Current.DisplayAlertAsync("Success", "Account created successfully!", "OK");
                    await Shell.Current.GoToAsync("//MainPage");
                }
                else
                {
                    ErrorMessage = "Registration failed. An account with this email may already exist.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Registration failed. Please check your connection.";
                System.Diagnostics.Debug.WriteLine($"[RegisterViewModel Error] {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task GoToLoginAsync()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}