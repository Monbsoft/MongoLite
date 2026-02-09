namespace Monbsoft.MongoLite.MApp.Models;

public class MongoCollectionInfo
{
    public string Name { get; set; } = string.Empty;
    public int DocumentCount { get; set; }
    public DateTime LastAccessed { get; set; } = DateTime.Now;
}