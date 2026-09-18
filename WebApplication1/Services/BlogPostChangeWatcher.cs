using WebApplication1.Configuration;
using WebApplication1.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebApplication1.Services
{
    public sealed class BlogPostChangeWatcher: BackgroundService
    {
        private readonly
 IMongoCollection<BlogPostDocument> _posts;
        private readonly MongoActivityLogService
        _activityLogService;
        private readonly ILogger<BlogPostChangeWatcher>
        _logger;
        public BlogPostChangeWatcher(
        IMongoClient mongoClient,
        MongoDbSettings settings,
        MongoActivityLogService activityLogService,
        ILogger<BlogPostChangeWatcher> logger)
        {
            IMongoDatabase database =
            mongoClient.GetDatabase(
            settings.DatabaseName);
            _posts =
            database.GetCollection<BlogPostDocument>(
            settings.CollectionName);
            _activityLogService =
            activityLogService;
            _logger =
            logger;
        }
        protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await WatchChangesAsync(
                    stoppingToken);
                }
                catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception exception)
                {
                    _logger.LogError(
                    exception,
                   "The MongoDB change stream stopped unexpectedly.");
                    await Task.Delay(
                    TimeSpan.FromSeconds(5),
                   stoppingToken);
                }
            }
        }
        private async Task WatchChangesAsync(
        CancellationToken cancellationToken)
        {
            ChangeStreamOptions options = new()
            {
                //For an update event, this asks MongoDB to retrieve the current version of the changed document
                FullDocument =
            ChangeStreamFullDocumentOption.UpdateLookup
            };
            using IChangeStreamCursor<
            ChangeStreamDocument<BlogPostDocument>>
            cursor =
            await _posts.WatchAsync(
            options: options,
            cancellationToken:
            cancellationToken);
            _logger.LogInformation(
            "MongoDB BlogPosts change stream started.");
            while (
            await cursor.MoveNextAsync(
            cancellationToken))
            {
                foreach (
                ChangeStreamDocument<BlogPostDocument>
                change
                in cursor.Current)
                {
                    await ProcessChangeAsync(
                    change,
                   cancellationToken);
                }
            }
        }
        private async Task ProcessChangeAsync(
        ChangeStreamDocument<BlogPostDocument> change,
        CancellationToken cancellationToken)
        {
            string operation =
            change.OperationType.ToString();
            string? blogPostId = null;
            if (change.DocumentKey != null &&
            change.DocumentKey.TryGetValue(
            "_id",
           out BsonValue idValue))
            {
                blogPostId =
                idValue.ToString();
            }
            // retrieve the title after an update
            string? title =
            change.FullDocument?.Title;
            DateTime now =
            DateTime.UtcNow;
            ActivityLogDocument activityLog = new()
            {
                OperationType =
            operation,
                BlogPostId =
            blogPostId,
                Title =
            title,
                Message =
            $"Blog post event detected: {operation}",
                CreatedAtUtc =
            now,
                ExpiresAtUtc =
            now.AddMinutes(2)
            };
            await _activityLogService.InsertAsync(
            activityLog,
            cancellationToken);
            _logger.LogInformation(
            "MongoDB change detected. " +
            "Operation: {OperationType}, " +
            "BlogPostId: {BlogPostId}",
            operation,
            blogPostId);
        }

    }
}
