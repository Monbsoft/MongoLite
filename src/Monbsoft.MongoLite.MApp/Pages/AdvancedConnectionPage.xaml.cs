using Monbsoft.MongoLite.MApp.ViewModels;

namespace Monbsoft.MongoLite.MApp.Pages;

public partial class AdvancedConnectionPage : ContentPage
{
    public AdvancedConnectionPage(AdvancedConnectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}
