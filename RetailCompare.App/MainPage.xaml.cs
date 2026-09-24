using RetailCompare.App.ViewModels;

namespace RetailCompare.App;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is ProductListViewModel vm)
        {
            await vm.LoadProductsAsync();
        }
    }
}