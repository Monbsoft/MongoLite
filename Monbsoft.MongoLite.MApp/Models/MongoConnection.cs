namespace Monbsoft.MongoLite.MApp.Models;

public class MongoConnection
{
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime ConnectedAt { get; set; }
    public string? SelectedDatabase { get; set; }
    public EnvironmentType Environment { get; set; } = EnvironmentType.Development;

    public static string DefaultDockerConnection => "mongodb://admin:password@localhost:27017/testdb";
}