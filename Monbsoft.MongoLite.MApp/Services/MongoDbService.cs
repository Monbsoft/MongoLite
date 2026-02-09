using MongoDB.Bson;
using MongoDB.Driver;
using Monbsoft.MongoLite.MApp.Models;

namespace Monbsoft.MongoLite.MApp.Services;

public class MongoDbService
{
    private IMongoDatabase? _database;
    private IMongoClient? _client;

    public bool IsConnected => _database != null;

    public async Task<bool> ConnectAsync(string connectionString)
    {
        try
        {
            var mongoUrl = MongoUrl.Create(connectionString);
            if (string.IsNullOrWhiteSpace(mongoUrl.DatabaseName))
                throw new ArgumentException("Connection string must include a database name (e.g. mongodb://host:port/mydb).");

            MongoClientSettings settings = MongoClientSettings.FromConnectionString(connectionString);           
            settings.DirectConnection = true;

            _client = new MongoClient(settings);
            _database = _client.GetDatabase(mongoUrl.DatabaseName);

            // Ping the database to verify connection
            await _database.RunCommandAsync((MongoDB.Driver.Command<BsonDocument>)"{ping:1}");

            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MongoDB connection failed: {ex.Message}");
            return false;
        }
    }

    public async Task<List<MongoCollectionInfo>> GetCollectionsAsync()
    {
        if (_database == null)
            throw new InvalidOperationException("Not connected to database");

        try
        {
            var cursor = await _database.ListCollectionNamesAsync();
            var collectionNames = await cursor.ToListAsync();
            var collectionInfos = new List<MongoCollectionInfo>();

            foreach (var collectionName in collectionNames)
            {
                var collection = _database.GetCollection<BsonDocument>(collectionName);
                var count = await collection.CountDocumentsAsync(FilterDefinition<BsonDocument>.Empty);

                collectionInfos.Add(new MongoCollectionInfo
                {
                    Name = collectionName,
                    DocumentCount = (int)count
                });
            }

            return collectionInfos.OrderBy(c => c.Name).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get collections: {ex.Message}");
            throw;
        }
    }

    public async Task<List<MongoDocumentInfo>> GetDocumentsAsync(string collectionName, int skip = 0, int limit = 50)
    {
        if (_database == null)
            throw new InvalidOperationException("Not connected to database");

        try
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var filter = FilterDefinition<BsonDocument>.Empty;
            var documents = await collection
                .Find(filter)
                .Skip(skip)
                .Limit(limit)
                .ToListAsync();

            return documents.Select(doc => new MongoDocumentInfo
            {
                Id = doc["_id"].ToString() ?? string.Empty,
                Json = doc.ToJson(),
                Preview = GetPreview(doc)
            }).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get documents: {ex.Message}");
            throw;
        }
    }

    public async Task<MongoDocumentInfo?> GetDocumentAsync(string collectionName, string documentId)
    {
        if (_database == null)
            throw new InvalidOperationException("Not connected to database");

        try
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", ParseId(documentId));
            var document = await collection.Find(filter).FirstOrDefaultAsync();

            return document != null ? new MongoDocumentInfo
            {
                Id = document["_id"].ToString() ?? string.Empty,
                Json = document.ToJson()
            } : null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get document: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> SaveDocumentAsync(string collectionName, string documentId, string json)
    {
        if (_database == null)
            throw new InvalidOperationException("Not connected to database");

        try
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var parsedId = ParseId(documentId);
            var filter = Builders<BsonDocument>.Filter.Eq("_id", parsedId);
            var replacement = BsonDocument.Parse(json);
            replacement["_id"] = parsedId;

            var result = await collection.ReplaceOneAsync(filter, replacement);
            return result.MatchedCount > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save document: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> DeleteDocumentAsync(string collectionName, string documentId)
    {
        if (_database == null)
            throw new InvalidOperationException("Not connected to database");

        try
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var filter = Builders<BsonDocument>.Filter.Eq("_id", ParseId(documentId));
            var result = await collection.DeleteOneAsync(filter);

            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to delete document: {ex.Message}");
            throw;
        }
    }

    private static BsonValue ParseId(string documentId)
    {
        if (ObjectId.TryParse(documentId, out var objectId))
            return objectId;
        return new BsonString(documentId);
    }

    private static string GetPreview(BsonDocument document)
    {
        try
        {
            var previewElements = new List<string>();

            if (document.Contains("name") || document.Contains("title"))
                previewElements.Add((document.Contains("name") ? document["name"].ToString() : document["title"].ToString()) ?? string.Empty);

            if (document.Contains("email"))
                previewElements.Add($"Email: {document["email"]}");

            if (document.Contains("price"))
                previewElements.Add($"Price: {document["price"]}");

            if (document.Contains("age"))
                previewElements.Add($"Age: {document["age"]}");

            if (previewElements.Count == 0)
                previewElements.Add($"ID: {document["_id"]}");

            return string.Join(" | ", previewElements.Take(3));
        }
        catch
        {
            return document["_id"].ToString() ?? string.Empty;
        }
    }
}
