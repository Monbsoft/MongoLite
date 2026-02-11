using System.Text.Json;
using Monbsoft.MongoLite.MApp.Models;

namespace Monbsoft.MongoLite.MApp.Services;

public class ConnectionStorageService
{
    private readonly string _filePath;
    private List<SavedConnection> _connections = new();

    public ConnectionStorageService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "connections.json");
    }

    public async Task<List<SavedConnection>> GetAllAsync()
    {
        await LoadAsync();
        return _connections.ToList();
    }

    public async Task SaveAsync(SavedConnection connection)
    {
        await LoadAsync();

        var existing = _connections.FindIndex(c => c.Id == connection.Id);
        if (existing >= 0)
            _connections[existing] = connection;
        else
            _connections.Add(connection);

        await PersistAsync();
    }

    public async Task DeleteAsync(string id)
    {
        await LoadAsync();
        _connections.RemoveAll(c => c.Id == id);
        await PersistAsync();
    }

    private async Task LoadAsync()
    {
        if (!File.Exists(_filePath))
        {
            _connections = new();
            return;
        }

        var json = await File.ReadAllTextAsync(_filePath);
        _connections = JsonSerializer.Deserialize<List<SavedConnection>>(json) ?? new();
    }

    private async Task PersistAsync()
    {
        var json = JsonSerializer.Serialize(_connections, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_filePath, json);
    }
}
