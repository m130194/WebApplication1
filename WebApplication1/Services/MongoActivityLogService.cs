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

    }
}
