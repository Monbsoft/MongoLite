namespace Monbsoft.MongoLite.MApp.Models;

public class SavedConnection
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string ConnectionString { get; set; } = string.Empty;
    public EnvironmentType Environment { get; set; } = EnvironmentType.Development;
}
