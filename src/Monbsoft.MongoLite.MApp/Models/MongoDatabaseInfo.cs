namespace Monbsoft.MongoLite.MApp.Models;

public class MongoDatabaseInfo
{
    public string Name { get; set; } = string.Empty;
    public int CollectionCount { get; set; }
    public DateTime LastAccessed { get; set; } = DateTime.Now;
}
