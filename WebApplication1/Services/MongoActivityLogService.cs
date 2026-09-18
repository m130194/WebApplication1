using WebApplication1.Configuration;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using MongoDB.Driver;

namespace WebApplication1.Services
{
    public sealed class MongoActivityLogService
    {
        private readonly
 IMongoCollection<ActivityLogDocument> _activityLogs;
        public MongoActivityLogService(
        IMongoClient mongoClient,
        MongoDbSettings settings)
        {
            IMongoDatabase database =
            mongoClient.GetDatabase(
            settings.DatabaseName);
            _activityLogs =
            database.GetCollection<ActivityLogDocument>(
            settings.ActivityLogCollectionName);
        }
        public async Task InsertAsync(
        ActivityLogDocument log,
        CancellationToken cancellationToken = default)
        {
            await _activityLogs.InsertOneAsync(
            log,
            cancellationToken:
            cancellationToken);
        }

        // Week 8 Part 31: Create the TTL Index

        public async Task CreateIndexesAsync(
 CancellationToken cancellationToken = default)
        {
            IndexKeysDefinition<ActivityLogDocument>
            ttlKeys =
            Builders<ActivityLogDocument>
            .IndexKeys
            .Ascending(
            log => log.ExpiresAtUtc);
            CreateIndexModel<ActivityLogDocument>
            ttlIndex =
            new(
            ttlKeys,
            new CreateIndexOptions
            {
                Name =
            "idx_activity_expiry_ttl",
                ExpireAfter =
            TimeSpan.Zero
            });
            await _activityLogs.Indexes
            .CreateOneAsync(
            ttlIndex,
            cancellationToken:
            cancellationToken);
        }


    }
}
