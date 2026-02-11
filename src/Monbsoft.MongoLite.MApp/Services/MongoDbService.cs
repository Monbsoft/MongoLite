using MongoDB.Bson;
using MongoDB.Driver;
using Monbsoft.MongoLite.MApp.Models;

namespace Monbsoft.MongoLite.MApp.Services;

public class MongoDbService
{
    private IMongoDatabase? _database;
    private IMongoClient? _client;
    private string? _selectedDatabaseName;

    public bool IsConnected => _database != null;
    public string? SelectedDatabase => _selectedDatabaseName;

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

    public async Task<List<MongoDatabaseInfo>> GetDatabasesAsync()
    {
        if (_client == null)
            throw new InvalidOperationException("Not connected to MongoDB");

        try
        {
            var databaseNames = await _client.ListDatabaseNamesAsync();
            var databaseList = await databaseNames.ToListAsync();
            var databaseInfos = new List<MongoDatabaseInfo>();

            foreach (var dbName in databaseList)
            {
                var db = _client.GetDatabase(dbName);
                var collectionNames = await db.ListCollectionNamesAsync();
                var collectionList = await collectionNames.ToListAsync();

                databaseInfos.Add(new MongoDatabaseInfo
                {
                    Name = dbName,
                    CollectionCount = collectionList.Count
                });
            }

            return databaseInfos.OrderBy(d => d.Name).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get databases: {ex.Message}");
            throw;
        }
    }

    public void SelectDatabase(string databaseName)
    {
        if (_client == null)
            throw new InvalidOperationException("Not connected to MongoDB");

        _database = _client.GetDatabase(databaseName);
        _selectedDatabaseName = databaseName;
    }

    public async Task<List<MongoDocumentInfo>> GetDocumentsAsync(string collectionName, int skip = 0, int limit = 50, string? filterJson = null, string? sortField = null, bool sortAscending = true)
    {
        if (_database == null)
            throw new InvalidOperationException("Not connected to database");

        try
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);

            var filter = ParseFilter(filterJson);
            var findFluent = collection.Find(filter);

            if (!string.IsNullOrWhiteSpace(sortField))
            {
                var sortDefinition = sortAscending
                    ? Builders<BsonDocument>.Sort.Ascending(sortField)
                    : Builders<BsonDocument>.Sort.Descending(sortField);
                findFluent = findFluent.Sort(sortDefinition);
            }

            var documents = await findFluent
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

    public async Task<long> CountDocumentsAsync(string collectionName, string? filterJson = null)
    {
        if (_database == null)
            throw new InvalidOperationException("Not connected to database");

        var collection = _database.GetCollection<BsonDocument>(collectionName);
        var filter = ParseFilter(filterJson);
        return await collection.CountDocumentsAsync(filter);
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

    private static FilterDefinition<BsonDocument> ParseFilter(string? filterJson)
    {
        if (string.IsNullOrWhiteSpace(filterJson))
            return FilterDefinition<BsonDocument>.Empty;

        var bsonFilter = BsonDocument.Parse(filterJson);
        return new BsonDocumentFilterDefinition<BsonDocument>(bsonFilter);
    }

    private static BsonValue ParseId(string documentId)
    {
        if (ObjectId.TryParse(documentId, out var objectId))
            return objectId;
        return new BsonString(documentId);
    }

    private static string GetPreview(BsonDocument document, int maxKeys = 4, int maxValueLength = 50)
    {
        try
        {
            var lines = new List<string>();

            // _id always first
            if (document.Contains("_id"))
            {
                var idValue = Truncate(document["_id"].ToString() ?? string.Empty, maxValueLength);
                lines.Add($"_id: {idValue}");
            }

            // Next keys in document order, skip _id
            foreach (var element in document.Elements)
            {
                if (lines.Count >= maxKeys)
                    break;

                if (element.Name == "_id")
                    continue;

                var value = Truncate(element.Value.ToString() ?? string.Empty, maxValueLength);
                lines.Add($"{element.Name}: {value}");
            }

            return string.Join("\n", lines);
        }
        catch
        {
            return document["_id"].ToString() ?? string.Empty;
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (value.Length <= maxLength)
            return value;
        return value[..maxLength] + "...";
    }
}
