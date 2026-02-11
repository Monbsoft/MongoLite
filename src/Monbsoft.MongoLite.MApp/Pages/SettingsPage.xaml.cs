using Monbsoft.MongoLite.MApp.ViewModels;

namespace Monbsoft.MongoLite.MApp.Pages;

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
