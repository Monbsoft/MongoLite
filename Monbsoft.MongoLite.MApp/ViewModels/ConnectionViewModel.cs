using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MongoLite.MApp.Models;
using Monbsoft.MongoLite.MApp.Services;

namespace Monbsoft.MongoLite.MApp.ViewModels;

public partial class ConnectionViewModel : ObservableObject
{
    private readonly MongoDbService _mongoDbService;

    private string _connectionString = MongoConnection.DefaultDockerConnection;
    private bool _isConnected;
    private bool _isConnecting;
    private string _errorMessage = string.Empty;

    public string ConnectionString
    {
        get => _connectionString;
        set => SetProperty(ref _connectionString, value);
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

    public IAsyncRelayCommand ConnectCommand { get; }
    public IAsyncRelayCommand TestConnectionCommand { get; }

    public ConnectionViewModel(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
        
        ConnectCommand = new AsyncRelayCommand(ConnectAsync);
        TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync);
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
            
            // Restore original state after test
            await Task.Delay(2000); // Show success message for 2 seconds
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
}