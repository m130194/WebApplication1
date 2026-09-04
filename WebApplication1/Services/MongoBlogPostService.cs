using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Configuration;
using WebApplication1.Models;


namespace WebApplication1.Services
{
    //This service separates database operations from controller code
    public sealed class MongoBlogPostService
    {
        private readonly IMongoCollection<BlogPostDocument> _posts;
        private readonly ILogger<MongoBlogPostService> _logger;
        public MongoBlogPostService(
        IMongoClient mongoClient,
        MongoDbSettings settings,
        ILogger<MongoBlogPostService> logger)
        {
            IMongoDatabase database =
            mongoClient.GetDatabase(settings.DatabaseName);
            _posts =
            database.GetCollection<BlogPostDocument>(
            settings.CollectionName);
            _logger = logger;
        }
        public async Task<List<BlogPostDocument>> GetAllAsync(
        CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
            "Retrieving blog post documents from MongoDB.");
            return await _posts
            .Find(Builders<BlogPostDocument>.Filter.Empty)
            .SortByDescending(post => post.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        }
        public async Task<BlogPostDocument?> GetByIdAsync(
        string id,
        CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return null;
            }
            _logger.LogInformation(
            "Retrieving MongoDB blog post {PostId}.",
            id);
            return await _posts
            .Find(post => post.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
        }
        public async Task<BlogPostDocument> InsertAsync(
        BlogPostDocument document,
        CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
            "Inserting blog post {PostTitle} into MongoDB.",
            document.Title);
            await _posts.InsertOneAsync(
            document,
            cancellationToken: cancellationToken);
            return document;
        }

        public async Task InsertManyAsync(IEnumerable<BlogPostDocument> documents, CancellationToken cancellationToken = default)
        {
            await _posts.InsertManyAsync(
            documents,
            cancellationToken: cancellationToken);
        }

        public async Task<bool> IncrementViewCountAsync(string id, CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;

            }

            FilterDefinition<BlogPostDocument> filter = Builders<BlogPostDocument>.Filter.Eq(post => post.Id, id);
            UpdateDefinition<BlogPostDocument> update =
            Builders<BlogPostDocument>.Update
            .Inc(post => post.ViewCount, 1);
            UpdateResult result =
            await _posts.UpdateOneAsync(
            filter,
            update,
            cancellationToken: cancellationToken);
            return result.MatchedCount > 0;
        }

        public async Task<long> PublishByTagAsync(
 string tag,
 CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter
            .AnyEq(post => post.Tags, tag);
            UpdateDefinition<BlogPostDocument> update =
            Builders<BlogPostDocument>.Update
            .Set(post => post.IsPublished, true)
            .Set(post => post.PublishedAtUtc, DateTime.UtcNow);
            UpdateResult result =
            await _posts.UpdateManyAsync(
            filter,
            update,
            cancellationToken: cancellationToken);
            return result.ModifiedCount;
        }

    }

}
