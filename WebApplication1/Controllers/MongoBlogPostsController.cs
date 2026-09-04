using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    //ASP.NET Core automatically returns a validation response when the submitted model is invalid
    [ApiController]
    [Route("api/mongo-blog-posts")]
    public sealed class MongoBlogPostsController : Controller
    {
        private readonly MongoBlogPostService _service;
        public MongoBlogPostsController(
        MongoBlogPostService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<BlogPostDocument>>> GetAll(
        CancellationToken cancellationToken)
        {
            List<BlogPostDocument> posts =
                await _service.GetAllAsync(cancellationToken);
            return Ok(posts);
        }

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<BlogPostDocument>> GetById(
        string id,
        CancellationToken cancellationToken)
        {
            BlogPostDocument? post =
            await _service.GetByIdAsync(
            id,
           cancellationToken);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        public async Task<ActionResult<BlogPostDocument>> Create(
        [FromBody] MongoBlogPostCreateViewModel viewModel,
        CancellationToken cancellationToken)
        {
            BlogPostDocument document = new()
            {
                Title = viewModel.Title.Trim(),
                Content = viewModel.Content.Trim(),
                Author = new AuthorDocument
                {
                    AuthorId = viewModel.AuthorId.Trim(),
                    Name = viewModel.AuthorName.Trim(),
                    Email = viewModel.AuthorEmail.Trim()
                },
                Tags = viewModel.Tags,
                ViewCount = 0,
                IsPublished = viewModel.IsPublished,
                CreatedAtUtc = DateTime.UtcNow,
                PublishedAtUtc =
            viewModel.IsPublished
            ? DateTime.UtcNow
            : null
            };
            await _service.InsertAsync(
            document,
            cancellationToken);
            return CreatedAtAction(
            nameof(GetById),
            new { id = document.Id },
            document);
        }

        [HttpPost("seed")]
        public async Task<IActionResult> Seed(
         CancellationToken cancellationToken)
        {
            List<BlogPostDocument> samplePosts =
            [
            new BlogPostDocument
                     {
                     Title = "MVC Controllers",
                     Content = "Controllers receive requests and select application responses.",
                     Author = new AuthorDocument
                     {
                     Name = "Sam Chen",
                     Email = "sam@example.com"
                     },
                     Tags = ["mvc", "aspnet"],
                     ViewCount = 20,
                     IsPublished = true,
                     CreatedAtUtc = DateTime.UtcNow,
                     PublishedAtUtc = DateTime.UtcNow
                     },
                     new BlogPostDocument
                     {
                     Title = "MongoDB Documents",
                     Content = "MongoDB stores data using flexible BSON documents.",
                     Author = new AuthorDocument
                     {
                     Name = "Sam Chen",
                     Email = "sam@example.com"
                     },

                     Tags = ["mongodb", "nosql"],
                     ViewCount = 14,
                     IsPublished = true,
                     CreatedAtUtc = DateTime.UtcNow,
                     PublishedAtUtc = DateTime.UtcNow
                     },
                     new BlogPostDocument
                     {
                     Title = "Future Article",
                     Content = "This document represents an unpublished draft blog post.",
                     Author = new AuthorDocument
                     {
                     Name = "Taylor Singh",
                     Email = "taylor@example.com"
                     },
                     Tags = ["mongodb", "draft"],
                     ViewCount = 0,
                     IsPublished = false,
                     CreatedAtUtc = DateTime.UtcNow,
                     PublishedAtUtc = null
                     }
             ];
            await _service.InsertManyAsync(
            samplePosts,
            cancellationToken);
            return Ok(new
            {
                inserted = samplePosts.Count
            });
        }

        [HttpPut("{id:length(24)}/view")]
        public async Task<IActionResult> AddView(string id, CancellationToken cancellationToken)
        {
            bool updated =
            await _service.IncrementViewCountAsync(
            id,
            cancellationToken);
            if (!updated)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPut("publish-by-tag/{tag}")]
        public async Task<IActionResult> PublishByTag(
 string tag,
 CancellationToken cancellationToken)

        {
            long modified =
            await _service.PublishByTagAsync(
            tag,
            cancellationToken);
            return Ok(new
            {
                tag,
                modified
            });
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(
 string id,
 CancellationToken cancellationToken)
        {
            bool deleted =
            await _service.DeleteByIdAsync(
            id,
            cancellationToken);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("drafts")]
        public async Task<IActionResult> DeleteDrafts(
 CancellationToken cancellationToken)
        {
            long deleted =
            await _service.DeleteDraftsAsync(
            cancellationToken);
            return Ok(new
            {
                deleted
            });
        }

        //Week 7 Part 9 to query test data
        [HttpPost("seed-query-data")]
        public async Task<IActionResult> SeedQueryData(
 CancellationToken cancellationToken)
        {
            await _service.InsertQueryTestDataAsync(
            300,
            cancellationToken);
            return Ok(new
            {
                inserted = 300
            });
        }

        //Get by category endpoint
        [HttpGet("category/{category}")]
        public async Task<ActionResult<List<BlogPostDocument>>>
        GetByCategory(string category, [FromQuery] int limit = 10, CancellationToken cancellationToken = default)
        //Get all posts by category
        //{
        //    List<BlogPostDocument> posts =
        //    await _service.GetPublishedByCategoryAsync(category, cancellationToken);
        //    return Ok(posts);
        //}

        //Limit Results
        {
            limit = Math.Clamp(limit, 1, 100);
            List<BlogPostDocument> posts =
            await _service.GetPublishedByCategoryAsync(
            category,
            limit,
            cancellationToken);
            return Ok(posts);
        }

        //Retrieve posts by author
        [HttpGet("author/{authorId}")]
        public async Task<ActionResult<List<BlogPostDocument>>>
         GetByAuthor(
         string authorId,
         CancellationToken cancellationToken)
        {
            List<BlogPostDocument> posts =
            await _service.GetByAuthorAsync(
            authorId,
            cancellationToken);
            return Ok(posts);
        }

        //Week 7 Part 15 GetPopularPosts with query
        [HttpGet("popular")]
        public async Task<ActionResult<List<BlogPostDocument>>> GetPopularPosts([FromQuery] int viewCount, CancellationToken cancellationToken)
        {
            List<BlogPostDocument> posts =
            await _service.GetPopularPostsAsync(
            viewCount,
            cancellationToken);
            return Ok(posts);
        }

        //Week 7 Part 17 projection query selects fields required by the application
        [HttpGet("category/{category}/summary")]
        public async Task<
         ActionResult<List<BlogPostSummaryViewModel>>>
         GetCategorySummary(
         string category,
         [FromQuery] int limit = 10,
         CancellationToken cancellationToken = default)
        {
            limit = Math.Clamp(limit, 1, 100);
            List<BlogPostSummaryViewModel> posts =
            await _service.GetCategorySummariesAsync(
            category,
            limit,
            cancellationToken);
            return Ok(posts);
        }



    }
}
