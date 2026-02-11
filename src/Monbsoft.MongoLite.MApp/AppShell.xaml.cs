using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.Pages;

namespace Monbsoft.MongoLite.MApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("collections", typeof(CollectionsPage));
        Routing.RegisterRoute("documents", typeof(DocumentsPage));
        Routing.RegisterRoute("document-detail", typeof(DocumentDetailPage));
        Routing.RegisterRoute("settings", typeof(SettingsPage));
    }

    public void UpdateEnvironmentBanner(EnvironmentType environment)
    {
        EnvironmentBanner.IsVisible = true;
        EnvironmentBanner.BackgroundColor = Color.FromArgb(environment.GetColor());
        EnvironmentLabel.Text = environment.GetDisplayName();
    }

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("settings");
    }
}
