using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.Services;

namespace Monbsoft.MongoLite.MApp.ViewModels;

public partial class CollectionsViewModel : ObservableObject
{
    private readonly MongoDbService _mongoDbService;

    private List<MongoCollectionInfo> _collections = new();
    private bool _isLoading;
    private string _errorMessage = string.Empty;

    public List<MongoCollectionInfo> Collections
    {
        get => _collections;
        private set => SetProperty(ref _collections, value);
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

    public CollectionsViewModel(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
        RefreshCommand = new AsyncRelayCommand(LoadCollectionsAsync);
    }

    public async Task LoadCollectionsAsync()
    {
        if (!_mongoDbService.IsConnected)
        {
            ErrorMessage = "Not connected to database. Please connect first.";
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var collections = await _mongoDbService.GetCollectionsAsync();
            Collections = collections;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load collections: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task InitializeAsync()
    {
        if (_mongoDbService.IsConnected)
        {
            await LoadCollectionsAsync();
        }
    }
}
