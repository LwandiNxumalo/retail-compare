using RetailCompare.App.ViewModels;

namespace RetailCompare.App;

public partial class MainPage : ContentPage
{
    private readonly ProductListViewModel _viewModel;

    public MainPage(ProductListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProductsAsync();
    }
}