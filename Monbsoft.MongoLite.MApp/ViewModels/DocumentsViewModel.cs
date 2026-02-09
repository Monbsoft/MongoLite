using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.Services;

namespace Monbsoft.MongoLite.MApp.ViewModels;

public partial class DocumentsViewModel : ObservableObject
{
    private readonly MongoDbService _mongoDbService;

    private string _collectionName = string.Empty;
    private List<MongoDocumentInfo> _documents = new();
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    public string CollectionName
    {
        get => _collectionName;
        set => SetProperty(ref _collectionName, value);
    }

    public List<MongoDocumentInfo> Documents
    {
        get => _documents;
        private set => SetProperty(ref _documents, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand<MongoDocumentInfo> SelectDocumentCommand { get; }

    public DocumentsViewModel(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
        
        RefreshCommand = new AsyncRelayCommand(LoadDocumentsAsync);
        SelectDocumentCommand = new AsyncRelayCommand<MongoDocumentInfo>(SelectDocumentAsync);
    }

    public async Task LoadDocumentsAsync()
    {
        if (string.IsNullOrWhiteSpace(CollectionName) || !_mongoDbService.IsConnected)
        {
            ErrorMessage = "Collection name is required and must be connected to database.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var documents = await _mongoDbService.GetDocumentsAsync(CollectionName);
            Documents = documents;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load documents: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task SelectDocumentAsync(MongoDocumentInfo document)
    {
        if (document == null)
            return;

        // Navigate to document detail page with the selected document
        await Shell.Current.GoToAsync($"//document-detail?collectionName={CollectionName}&documentId={document.Id}");
    }

    public async Task InitializeAsync(string collectionName)
    {
        CollectionName = collectionName;
        
        if (_mongoDbService.IsConnected)
        {
            await LoadDocumentsAsync();
        }
    }
}