namespace Monbsoft.MongoLite.MApp.Models;

public class MongoConnection
{
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ConnectedAt { get; set; }
    
    public static string DefaultDockerConnection => "mongodb://admin:password@localhost:27017/testdb";
}