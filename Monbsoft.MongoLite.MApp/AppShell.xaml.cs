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
    }
}
