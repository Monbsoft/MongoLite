using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.Pages;
using Monbsoft.MongoLite.MApp.Services;

namespace Monbsoft.MongoLite.MApp.ViewModels;

public partial class ConnectionViewModel : ObservableObject
{
    private readonly MongoDbService _mongoDbService;
    private readonly ConnectionStorageService _storageService;
    private readonly IServiceProvider _serviceProvider;

    private string _connectionString = MongoConnection.DefaultDockerConnection;
    private string _connectionName = string.Empty;
    private EnvironmentType _selectedEnvironment = EnvironmentType.Development;
    private bool _isConnected;
    private bool _isConnecting;
    private string _errorMessage = string.Empty;
    private SavedConnection? _selectedSavedConnection;

    public string ConnectionString
    {
        get => _connectionString;
        set => SetProperty(ref _connectionString, value);
    }

    public string ConnectionName
    {
        get => _connectionName;
        set => SetProperty(ref _connectionName, value);
    }

    public EnvironmentType SelectedEnvironment
    {
        get => _selectedEnvironment;
        set => SetProperty(ref _selectedEnvironment, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        private set => SetProperty(ref _isConnected, value);
    }

    public bool IsConnecting
    {
        get => _isConnecting;
        private set => SetProperty(ref _isConnecting, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        private set => SetProperty(ref _errorMessage, value);
    }

    public SavedConnection? SelectedSavedConnection
    {
        get => _selectedSavedConnection;
        set
        {
            if (SetProperty(ref _selectedSavedConnection, value) && value != null)
            {
                ConnectionString = value.ConnectionString;
                ConnectionName = value.Name;
                SelectedEnvironment = value.Environment;
            }
        }
    }

    public ObservableCollection<SavedConnection> SavedConnections { get; } = new();
    public List<EnvironmentType> EnvironmentTypes { get; } = Enum.GetValues<EnvironmentType>().ToList();

    public IAsyncRelayCommand ConnectCommand { get; }
    public IAsyncRelayCommand TestConnectionCommand { get; }
    public IAsyncRelayCommand OpenAdvancedFormCommand { get; }
    public IAsyncRelayCommand SaveConnectionCommand { get; }
    public IAsyncRelayCommand DeleteConnectionCommand { get; }
    public IAsyncRelayCommand LoadSavedConnectionsCommand { get; }

    public ConnectionViewModel(MongoDbService mongoDbService, ConnectionStorageService storageService, IServiceProvider serviceProvider)
    {
        _mongoDbService = mongoDbService;
        _storageService = storageService;
        _serviceProvider = serviceProvider;

        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
        TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync);
        OpenAdvancedFormCommand = new AsyncRelayCommand(OpenAdvancedFormAsync);
        SaveConnectionCommand = new AsyncRelayCommand(SaveConnectionAsync);
        DeleteConnectionCommand = new AsyncRelayCommand(DeleteConnectionAsync);
        LoadSavedConnectionsCommand = new AsyncRelayCommand(LoadSavedConnectionsAsync);
    }

    public async Task LoadSavedConnectionsAsync()
    {
        var connections = await _storageService.GetAllAsync();
        SavedConnections.Clear();
        foreach (var conn in connections)
            SavedConnections.Add(conn);
    }

    private async Task SaveConnectionAsync()
    {
        if (string.IsNullOrWhiteSpace(ConnectionName))
        {
            ErrorMessage = "Connection name is required.";
            return;
        }

        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            ErrorMessage = "Connection string is required.";
            return;
        }

        var connection = SelectedSavedConnection ?? new SavedConnection();
        connection.Name = ConnectionName;
        connection.ConnectionString = ConnectionString;
        connection.Environment = SelectedEnvironment;

        await _storageService.SaveAsync(connection);
        await LoadSavedConnectionsAsync();

        ErrorMessage = $"Connection '{ConnectionName}' saved.";
    }

    private async Task DeleteConnectionAsync()
    {
        if (SelectedSavedConnection == null)
        {
            ErrorMessage = "Select a connection to delete.";
            return;
        }

        await _storageService.DeleteAsync(SelectedSavedConnection.Id);
        SelectedSavedConnection = null;
        ConnectionName = string.Empty;
        await LoadSavedConnectionsAsync();
    }

    private async Task ConnectAsync()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            ErrorMessage = "Connection string cannot be empty";
            return;
        }

        IsConnecting = true;
        ErrorMessage = string.Empty;

        try
        {
            var success = await _mongoDbService.ConnectAsync(ConnectionString);
            IsConnected = success;

            if (success)
            {
                _mongoDbService.CurrentEnvironment = SelectedEnvironment;
                if (Shell.Current is AppShell appShell)
                    appShell.UpdateEnvironmentBanner(SelectedEnvironment);
                await Shell.Current.GoToAsync("collections");
            }
            else
            {
                ErrorMessage = "Failed to connect to MongoDB. Please check your connection string.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Connection error: {ex.Message}";
            IsConnected = false;
        }
        finally
        {
            IsConnecting = false;
        }
    }

    private async Task TestConnectionAsync()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            ErrorMessage = "Connection string cannot be empty for testing";
            return;
        }

        var originalIsConnected = IsConnected;
        var originalErrorMessage = ErrorMessage;

        IsConnecting = true;
        ErrorMessage = string.Empty;

        try
        {
            var success = await _mongoDbService.ConnectAsync(ConnectionString);
            ErrorMessage = success ? "Connection test successful!" : "Connection test failed. Please check your connection string.";

            await Task.Delay(2000);
            IsConnected = originalIsConnected;
            ErrorMessage = originalErrorMessage;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Connection test error: {ex.Message}";
        }
        finally
        {
            IsConnecting = false;
        }
    }

    private async Task OpenAdvancedFormAsync()
    {
        var advancedPage = _serviceProvider.GetRequiredService<AdvancedConnectionPage>();
        var advancedViewModel = _serviceProvider.GetRequiredService<AdvancedConnectionViewModel>();

        advancedViewModel.SetConnectionSuccessCallback((connectionString) =>
        {
            ConnectionString = connectionString;
        });

        await Application.Current!.MainPage!.Navigation.PushModalAsync(advancedPage);
    }
}
