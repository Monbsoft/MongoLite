using Monbsoft.MongoLite.MApp.ViewModels;

namespace Monbsoft.MongoLite.MApp.Pages;

public partial class ConnectionPage : ContentPage
{
    private readonly ConnectionViewModel _viewModel;

    public ConnectionPage(ConnectionViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSavedConnectionsAsync();
    }
}
