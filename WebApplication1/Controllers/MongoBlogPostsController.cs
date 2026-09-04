using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.ViewModels;

using Microsoft.AspNetCore.Mvc;

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


    }
}
