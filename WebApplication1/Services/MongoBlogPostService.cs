using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication1.Configuration;
using WebApplication1.Models;
using WebApplication1.ViewModels;


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

        public async Task<long> PublishByTagAsync(string tag, CancellationToken cancellationToken = default)
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

        public async Task<long> ChangeAuthorEmailAsync(
 string oldEmail,
 string newEmail,
 CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.Author.Email, oldEmail);
            UpdateDefinition<BlogPostDocument> update =
            Builders<BlogPostDocument>.Update
            .Set(post => post.Author.Email, newEmail);
            UpdateResult result =
            await _posts.UpdateManyAsync(
            filter,
            update,
            cancellationToken: cancellationToken);
            return result.ModifiedCount;
        }

        public async Task<bool> DeleteByIdAsync(
 string id,
 CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.Id, id);
            DeleteResult result =
            await _posts.DeleteOneAsync(
                filter,
 cancellationToken);
            return result.DeletedCount > 0;
        }

        //Week 7 Part 22 Delete Multiple Documents
        public async Task<long> DeleteDraftsByCategoryAsync(
 string category,
 CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter.And(
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.Category, category),
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.IsPublished, false)
            );
            DeleteResult result =
            await _posts.DeleteManyAsync(
            filter,
            cancellationToken);
            return result.DeletedCount;
        }


        public async Task<long> DeleteDraftsAsync(
 CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.IsPublished, false);
            DeleteResult result =
            await _posts.DeleteManyAsync(
            filter,
            cancellationToken);
            return result.DeletedCount;
        }

        //this is to create additional test data
        public async Task InsertQueryTestDataAsync(
 int count,
 CancellationToken cancellationToken = default)
        {
            string[] categories =
            [
            "Programming",
 "Databases",
 "DevOps",
 "Security"
            ];
            (string Id, string Name, string Email)[] authors =
            [
            ("AUTHOR-001", "Sam Chen", "sam@example.com"),
 ("AUTHOR-002", "Taylor Singh", "taylor@example.com"),
 ("AUTHOR-003", "Morgan Lee", "morgan@example.com"),
 ("AUTHOR-004", "Jordan Smith", "jordan@example.com")
            ];
            Random random = new(7);
            List<BlogPostDocument> documents = [];
            for (int i = 1; i <= count; i++)
            {
                string category =
                categories[i % categories.Length];
                var author =
                authors[i % authors.Length];
                bool published =
                i % 4 != 0;
                DateTime created =
                DateTime.UtcNow
                .AddDays(-(i % 120))
                .AddMinutes(-i);
                BlogPostDocument post = new()
                {
                    Title = $"Session 7 Sample Post {i}",
                    Content =
                $"This is sample blog post number {i} used for MongoDB query testing.",
                    Category = category,
                    Author = new AuthorDocument
                    {
                        AuthorId = author.Id,
                        Name = author.Name,
                        Email = author.Email
                    },
                    Tags =
                category == "Databases"
                ? ["mongodb", "nosql"]
               : category == "DevOps"
                ? ["docker", "devops"]
               : ["programming"],
                    ViewCount =
                random.Next(0, 1000),
                    IsPublished =
                published,
                    CreatedAtUtc =
                created,
                    PublishedAtUtc =
                published
                ? created.AddHours(1)
               : null
                };
                documents.Add(post);
            }
            await _posts.InsertManyAsync(
            documents,
            cancellationToken: cancellationToken);
        }
        //end test data

        //Week 7 Part 10 Read Multiple Documents Using a Filter
        public async Task<List<BlogPostDocument>>
        GetPublishedByCategoryAsync(string category, int limit, CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter.And(
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.Category, category),
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.IsPublished, true)
            );
            return await _posts
            .Find(filter)
            .SortByDescending(post => post.PublishedAtUtc) //sort results by newer posts first
            .Limit(limit) //limit results. this is where we added int limit to parameters 
            .ToListAsync(cancellationToken);
        }

        //Read Multiple Documents Using Filter Unpublished
        public async Task<List<BlogPostDocument>>
        GetUnpublishedByCategoryAsync(string category, int limit, CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter.And(
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.Category, category),
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.IsPublished, false)
            );
            return await _posts
            .Find(filter)
            .SortByDescending(post => post.PublishedAtUtc) //sort results by newer posts first
            .Limit(limit) //limit results. this is where we added int limit to parameters 
            .ToListAsync(cancellationToken);
        }

        //Week 7 Part 14 Retrieve Posts by Author
        public async Task<List<BlogPostDocument>>
         GetByAuthorAsync(
         string authorId,
         CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter
            .Eq(
            post => post.Author.AuthorId,
            authorId);
            return await _posts
            .Find(filter)
            .SortByDescending(post => post.PublishedAtUtc)
            .ToListAsync(cancellationToken);
        }

        //Week 7 Part 15 Create a Range Filter that returns popular published posts
        public async Task<List<BlogPostDocument>>
        GetPopularPostsAsync(int minimumViews, CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter.And(
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.IsPublished, true),
            Builders<BlogPostDocument>.Filter
            .Gte( //this .Gte operator means greater than or equal to
            post => post.ViewCount,
           minimumViews)
            );
            return await _posts
            .Find(filter)
            .SortByDescending(post => post.ViewCount)
            .ToListAsync(cancellationToken);
        }

        //Week 7 Part 16 Add projection with query that selects fields required by the application
        public async Task<List<BlogPostSummaryViewModel>>
         GetCategorySummariesAsync(
         string category,
         int limit,
         CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter.And(
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.Category, category),
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.IsPublished, true)
            );
            return await _posts
            .Find(filter)
            .SortByDescending(post => post.PublishedAtUtc)
            .Limit(limit)
            .Project(post =>
            new BlogPostSummaryViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Category = post.Category,
                AuthorName = post.Author.Name,
                ViewCount = post.ViewCount,
                PublishedAtUtc = post.PublishedAtUtc
            })
            .ToListAsync(cancellationToken);
        }

        //Week 7 Part 19 Update one document 
        public async Task<bool> UpdateAsync(
         string id,
         BlogPostUpdateViewModel viewModel,
         CancellationToken cancellationToken = default)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return false;
            }
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.Id, id);
            UpdateDefinition<BlogPostDocument> update =
            Builders<BlogPostDocument>.Update
            .Set(post => post.Title, viewModel.Title.Trim())
            .Set(post => post.Content, viewModel.Content.Trim())
            .Set(post => post.Category, viewModel.Category.Trim())
            .Set(post => post.Tags, viewModel.Tags)
            .Set(post => post.IsPublished, viewModel.IsPublished)
            .Set(
            post => post.PublishedAtUtc,
            viewModel.IsPublished
            ? DateTime.UtcNow
           : null);
            UpdateResult result =
            await _posts.UpdateOneAsync(
            filter,
            update,
            cancellationToken: cancellationToken);
            return result.MatchedCount > 0;
        }

        //Week 7 Part 20 Update multiple documents publishes all drafts by category
        public async Task<long> PublishCategoryAsync(
 string category,
 CancellationToken cancellationToken = default)
        {
            FilterDefinition<BlogPostDocument> filter =
            Builders<BlogPostDocument>.Filter.And(
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.Category, category),
            Builders<BlogPostDocument>.Filter
            .Eq(post => post.IsPublished, false)
            );
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

        //Week 7 Part 26 Access Patterns: Create the category compound index
        public async Task CreateIndexesAsync(
 CancellationToken cancellationToken = default)
        {
            IndexKeysDefinition<BlogPostDocument> categoryIndex =
            Builders<BlogPostDocument>.IndexKeys
            .Ascending(post => post.Category)
            .Ascending(post => post.IsPublished)
            .Descending(post => post.PublishedAtUtc);
            CreateIndexModel<BlogPostDocument> categoryIndexModel =
            new(
            categoryIndex,
            new CreateIndexOptions
            {
                Name =
            "idx_category_published_date"
            });
            await _posts.Indexes.CreateOneAsync(
            categoryIndexModel,
            cancellationToken: cancellationToken);

            //Retrieve posts belonging to one author, newest first
            IndexKeysDefinition<BlogPostDocument> authorIndex =
         Builders<BlogPostDocument>.IndexKeys
         .Ascending(post => post.Author.AuthorId)
         .Descending(post => post.PublishedAtUtc);
            CreateIndexModel<BlogPostDocument> authorIndexModel =
             new(
             authorIndex,
             new CreateIndexOptions
             {
                 Name =
             "idx_author_date"
             });
            await _posts.Indexes.CreateManyAsync(
             [
             categoryIndexModel,
             authorIndexModel
             ],
             cancellationToken);


        }


    }

}
