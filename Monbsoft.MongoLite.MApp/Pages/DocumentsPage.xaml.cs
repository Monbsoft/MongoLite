using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.ViewModels;

namespace Monbsoft.MongoLite.MApp.Pages;

[QueryProperty(nameof(CollectionName), "collectionName")]
public partial class DocumentsPage : ContentPage
{
    private readonly DocumentsViewModel _viewModel;

    public string CollectionName { get; set; } = string.Empty;

    public DocumentsPage(DocumentsViewModel viewModel)
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
            await _viewModel.InitializeAsync(CollectionName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"DocumentsPage.OnAppearing error: {ex.Message}");
        }
    }

    private async void OnDocumentSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is MongoDocumentInfo document)
        {
            if (sender is CollectionView cv)
                cv.SelectedItem = null;

            await Shell.Current.GoToAsync($"document-detail?collectionName={_viewModel.CollectionName}&documentId={document.Id}");
        }
    }
}
