using WebApplication1.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebApplication1.HealthChecks
{
    public sealed class MongoDbHealthCheck
 : IHealthCheck
    {
        private readonly IMongoClient
        _mongoClient;
        private readonly MongoDbSettings
        _settings;
        public MongoDbHealthCheck(
        IMongoClient mongoClient,
        MongoDbSettings settings)
        {
            _mongoClient =
            mongoClient;
            _settings =
            settings;
        }
        public async Task<HealthCheckResult>
        CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
        {
            try
            {
                IMongoDatabase database =
                _mongoClient.GetDatabase(
                _settings.DatabaseName);
                BsonDocumentCommand<BsonDocument>
                command =
               new(
                new BsonDocument(
                "ping",
               1));
                await database.RunCommandAsync(
                command,
                cancellationToken:
                cancellationToken);
                return HealthCheckResult.Healthy(
                "MongoDB is reachable.");
            }
            catch (Exception exception)
            {
                return HealthCheckResult.Unhealthy(
                "MongoDB is not reachable.",
               exception);
            }
        }
    }
}

