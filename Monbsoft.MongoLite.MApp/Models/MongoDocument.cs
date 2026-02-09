namespace Monbsoft.MongoLite.MApp.Models;

public class MongoDocumentInfo
{
    public string Id { get; set; } = string.Empty;
    public string Json { get; set; } = string.Empty;
    public string Preview { get; set; } = string.Empty;
    public DateTime LastModified { get; set; } = DateTime.Now;
}