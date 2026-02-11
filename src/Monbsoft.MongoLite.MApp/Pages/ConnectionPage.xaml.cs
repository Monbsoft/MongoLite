using Monbsoft.MongoLite.MApp.ViewModels;

namespace Monbsoft.MongoLite.MApp.Pages;

public partial class ConnectionPage : ContentPage
{
    public ConnectionPage(ConnectionViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
