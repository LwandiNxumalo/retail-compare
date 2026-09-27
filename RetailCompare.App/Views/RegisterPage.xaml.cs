using RetailCompare.App.ViewModels;

namespace RetailCompare.App.Views;
public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
