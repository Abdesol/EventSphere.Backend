using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace EventSphere.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    private IMongoCollection<GridFSFileInfo> _fileCollection;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
        _database = client.GetDatabase(configuration["MongoDbName"]);
        _fileCollection = _database.GetCollection<GridFSFileInfo>("fs.files");

        EnsureIndexes();
    }

    public IGridFSBucket GetGridFsBucket()
    {
        return new GridFSBucket(_fileCollection.Database);
    }

    private void EnsureIndexes()
    {
        var indexKeysBuilder = Builders<GridFSFileInfo>.IndexKeys;

        // Create an index on the Filename field
        var filenameIndex = new CreateIndexModel<GridFSFileInfo>(indexKeysBuilder.Ascending("filename"));
        _fileCollection.Indexes.CreateOne(filenameIndex);

        // Create an index on the _id field (which contains a timestamp)
        var idIndex = new CreateIndexModel<GridFSFileInfo>(indexKeysBuilder.Ascending("_id"));
        _fileCollection.Indexes.CreateOne(idIndex);
    }
}