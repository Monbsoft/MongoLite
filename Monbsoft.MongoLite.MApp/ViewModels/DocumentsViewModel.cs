using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.Services;

namespace Monbsoft.MongoLite.MApp.ViewModels;

public partial class DocumentsViewModel : ObservableObject
{
    private readonly MongoDbService _mongoDbService;
    private const int PageSize = 20;

    private string _collectionName = string.Empty;
    private List<MongoDocumentInfo> _documents = new();
    private bool _isLoading;
    private string _errorMessage = string.Empty;
    private int _currentPage = 1;
    private int _totalPages = 1;
    private long _totalDocuments;
    private string _filterQuery = string.Empty;
    private string _sortField = string.Empty;
    private bool _sortAscending = true;

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

    public int CurrentPage
    {
        get => _currentPage;
        private set
        {
            if (SetProperty(ref _currentPage, value))
            {
                OnPropertyChanged(nameof(PageInfo));
                OnPropertyChanged(nameof(CanGoPrevious));
                OnPropertyChanged(nameof(CanGoNext));
            }
        }
    }

    public int TotalPages
    {
        get => _totalPages;
        private set
        {
            if (SetProperty(ref _totalPages, value))
            {
                OnPropertyChanged(nameof(PageInfo));
                OnPropertyChanged(nameof(CanGoNext));
            }
        }
    }

    public long TotalDocuments
    {
        get => _totalDocuments;
        private set
        {
            if (SetProperty(ref _totalDocuments, value))
                OnPropertyChanged(nameof(DocumentCountInfo));
        }
    }

    public string FilterQuery
    {
        get => _filterQuery;
        set => SetProperty(ref _filterQuery, value);
    }

    public string SortField
    {
        get => _sortField;
        set => SetProperty(ref _sortField, value);
    }

    public bool SortAscending
    {
        get => _sortAscending;
        set
        {
            if (SetProperty(ref _sortAscending, value))
                OnPropertyChanged(nameof(SortDirectionIcon));
        }
    }

    public string SortDirectionIcon => SortAscending ? "▲ Asc" : "▼ Desc";

    public string PageInfo => $"Page {CurrentPage} / {TotalPages}";
    public string DocumentCountInfo => $"{TotalDocuments} document(s)";
    public bool CanGoPrevious => CurrentPage > 1;
    public bool CanGoNext => CurrentPage < TotalPages;

    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand PreviousPageCommand { get; }
    public IAsyncRelayCommand NextPageCommand { get; }
    public IAsyncRelayCommand ApplyFilterCommand { get; }
    public IAsyncRelayCommand ResetFilterCommand { get; }
    public IRelayCommand ToggleSortDirectionCommand { get; }

    public DocumentsViewModel(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
        RefreshCommand = new AsyncRelayCommand(LoadDocumentsAsync);
        PreviousPageCommand = new AsyncRelayCommand(GoToPreviousPageAsync);
        NextPageCommand = new AsyncRelayCommand(GoToNextPageAsync);
        ApplyFilterCommand = new AsyncRelayCommand(ApplyFilterAsync);
        ResetFilterCommand = new AsyncRelayCommand(ResetFilterAsync);
        ToggleSortDirectionCommand = new RelayCommand(() => SortAscending = !SortAscending);
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
            string? filter = string.IsNullOrWhiteSpace(FilterQuery) ? null : FilterQuery;
            TotalDocuments = await _mongoDbService.CountDocumentsAsync(CollectionName, filter);
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)TotalDocuments / PageSize));

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            int skip = (CurrentPage - 1) * PageSize;
            string? sort = string.IsNullOrWhiteSpace(SortField) ? null : SortField;
            Documents = await _mongoDbService.GetDocumentsAsync(CollectionName, skip, PageSize, filter, sort, SortAscending);
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

    private async Task GoToPreviousPageAsync()
    {
        if (CanGoPrevious)
        {
            CurrentPage--;
            await LoadDocumentsAsync();
        }
    }

    private async Task GoToNextPageAsync()
    {
        if (CanGoNext)
        {
            CurrentPage++;
            await LoadDocumentsAsync();
        }
    }

    private async Task ApplyFilterAsync()
    {
        CurrentPage = 1;
        await LoadDocumentsAsync();
    }

    private async Task ResetFilterAsync()
    {
        FilterQuery = string.Empty;
        CurrentPage = 1;
        await LoadDocumentsAsync();
    }

    public async Task InitializeAsync(string collectionName)
    {
        CollectionName = collectionName;
        CurrentPage = 1;
        FilterQuery = string.Empty;
        SortField = string.Empty;
        SortAscending = true;

        if (_mongoDbService.IsConnected)
        {
            await LoadDocumentsAsync();
        }
    }
}
