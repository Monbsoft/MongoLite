using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Monbsoft.MongoLite.MApp.Services;

namespace Monbsoft.MongoLite.MApp.ViewModels;

public partial class AdvancedConnectionViewModel : ObservableObject
{
    private readonly MongoDbService _mongoDbService;
    private Action<string>? _onConnectionSuccessCallback;

    [ObservableProperty]
    private string _host = "localhost";

    [ObservableProperty]
    private string _port = "27017";

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _database = string.Empty;

    [ObservableProperty]
    private string _authSource = "admin";

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isConnecting;

    public AdvancedConnectionViewModel(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
        _onConnectionSuccessCallback = null;
    }

    public void SetConnectionSuccessCallback(Action<string> callback)
    {
        _onConnectionSuccessCallback = callback;
    }

    [RelayCommand(CanExecute = nameof(CanConnect))]
    private async Task ConnectAsync()
    {
        IsConnecting = true;
        ErrorMessage = string.Empty;

        try
        {
            var builtConnectionString = BuildConnectionString();
            var success = await _mongoDbService.ConnectAsync(builtConnectionString);

            if (success)
            {
                _onConnectionSuccessCallback?.Invoke(builtConnectionString);
                await Application.Current.MainPage.Navigation.PopModalAsync();
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
        }
        finally
        {
            IsConnecting = false;
        }
    }

    private bool CanConnect()
    {
        if (string.IsNullOrWhiteSpace(Host) || Host.Contains(' '))
            return false;

        if (string.IsNullOrWhiteSpace(Database))
            return false;

        if (!int.TryParse(Port, out int portNumber))
            return false;

        if (portNumber < 1 || portNumber > 65535)
            return false;

        return true;
    }

    private string BuildConnectionString()
    {
        var auth = string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password)
            ? ""
            : $"{Username}:{Password}@";

        var authSourceParam = string.IsNullOrWhiteSpace(AuthSource)
            ? ""
            : $"?authSource={AuthSource}";

        return $"mongodb://{auth}{Host}:{Port}/{Database}{authSourceParam}";
    }

    partial void OnHostChanged(string value)
    {
        ConnectCommand.NotifyCanExecuteChanged();
    }

    partial void OnPortChanged(string value)
    {
        ConnectCommand.NotifyCanExecuteChanged();
    }

    partial void OnDatabaseChanged(string value)
    {
        ConnectCommand.NotifyCanExecuteChanged();
    }
}
