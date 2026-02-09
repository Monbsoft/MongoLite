using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.Services;
using System.Text.Json;

namespace Monbsoft.MongoLite.MApp.ViewModels;

public partial class DocumentDetailViewModel : ObservableObject
{
    private readonly MongoDbService _mongoDbService;

    private string _collectionName = string.Empty;
    private string _documentId = string.Empty;
    private string _json = string.Empty;
    private bool _isLoading;
    private bool _isModified;
    private string _errorMessage = string.Empty;

    public string CollectionName
    {
        get => _collectionName;
        set => SetProperty(ref _collectionName, value);
    }

    public string DocumentId
    {
        get => _documentId;
        set => SetProperty(ref _documentId, value);
    }

    public string Json
    {
        get => _json;
        set
        {
            SetProperty(ref _json, value);
            IsModified = true;
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        private set => SetProperty(ref _isLoading, value);
    }

    public bool IsModified
    {
        get => _isModified;
        private set => SetProperty(ref _isModified, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand DeleteCommand { get; }
    public IAsyncRelayCommand LoadCommand { get; }

    public DocumentDetailViewModel(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
        
        SaveCommand = new AsyncRelayCommand(SaveAsync);
        DeleteCommand = new AsyncRelayCommand(DeleteAsync);
        LoadCommand = new AsyncRelayCommand(LoadDocumentAsync);
    }

    public async Task LoadDocumentAsync()
    {
        if (string.IsNullOrWhiteSpace(CollectionName) || string.IsNullOrWhiteSpace(DocumentId) || !_mongoDbService.IsConnected)
        {
            ErrorMessage = "Collection name and Document ID are required and must be connected to database.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var document = await _mongoDbService.GetDocumentAsync(CollectionName, DocumentId);
            if (document != null)
            {
                Json = document.Json;
            }
            else
            {
                ErrorMessage = "Document not found";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load document: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Json))
        {
            ErrorMessage = "JSON cannot be empty when saving.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            // Validate JSON before saving
            try
            {
                JsonDocument.Parse(Json);
            }
            catch
            {
                ErrorMessage = "Invalid JSON format. Please check your syntax.";
                return;
            }

            var success = await _mongoDbService.SaveDocumentAsync(CollectionName, DocumentId, Json);
            
            if (success)
            {
                IsModified = false;
                ErrorMessage = "Document saved successfully!";
            }
            else
            {
                ErrorMessage = "Failed to save document. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Save error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeleteAsync()
    {
        if (string.IsNullOrWhiteSpace(CollectionName) || string.IsNullOrWhiteSpace(DocumentId))
        {
            ErrorMessage = "Collection name and Document ID are required for deletion.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var success = await _mongoDbService.DeleteDocumentAsync(CollectionName, DocumentId);
            
            if (success)
            {
                ErrorMessage = "Document deleted successfully!";
                // Navigate back to documents list
                await Shell.Current.GoToAsync($"//documents?collectionName={CollectionName}");
            }
            else
            {
                ErrorMessage = "Failed to delete document. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Delete error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task InitializeAsync(string collectionName, string documentId)
    {
        CollectionName = collectionName;
        DocumentId = documentId;
        IsModified = false;
        ErrorMessage = string.Empty;
        
        if (_mongoDbService.IsConnected)
        {
            await LoadDocumentAsync();
        }
    }
}