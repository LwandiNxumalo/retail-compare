using RetailCompare.App.ViewModels;

namespace RetailCompare.App.Views;

public partial class WatchlistPage : ContentPage
{
    private readonly WatchlistViewModel _viewModel;

    public WatchlistPage(WatchlistViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadWatchlistAsync();
    }
}