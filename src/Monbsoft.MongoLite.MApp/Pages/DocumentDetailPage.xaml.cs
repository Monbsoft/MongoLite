using Monbsoft.MongoLite.MApp.ViewModels;

namespace Monbsoft.MongoLite.MApp.Pages;

[QueryProperty(nameof(CollectionName), "collectionName")]
[QueryProperty(nameof(DocumentId), "documentId")]
public partial class DocumentDetailPage : ContentPage
{
    private readonly DocumentDetailViewModel _viewModel;

    public string CollectionName { get; set; } = string.Empty;
    public string DocumentId { get; set; } = string.Empty;

    public DocumentDetailPage(DocumentDetailViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            await _viewModel.InitializeAsync(CollectionName, DocumentId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DocumentDetailPage.OnAppearing error: {ex.Message}");
        }
    }
}
