using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.ViewModels;

namespace WebApplication1.Controllers
{
    
    public class BlogController : Controller
    {
        private readonly ILogger<BlogController> _logger;

        public BlogController(ILogger<BlogController> logger)
        {
            _logger = logger;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation("Displaying the blog index.");
            List<BlogPost> blogPosts = Posts
            .OrderByDescending(post => post.Id)
            .ToList();
            return View(blogPosts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private static readonly List<BlogPost> Posts =
        [
            new BlogPost
            {
                Id = 1,
                Title = "First Post",
                Content = "This is the content of the first post."
            },
            new BlogPost
            {
                Id = 2,
                Title = "Second Post",
                Content = "This is the content of the second post."
            }
        ];


        //Part 5: Add a GET Route for All Posts
        //Part 7: Add a Query-String Search
        [HttpGet("/api/blog")]
        public IActionResult GetAll([FromQuery] string? search)
        {
            _logger.LogInformation(
            "GET request received for all blog posts. Search term: { SearchTerm}", search);
            if (string.IsNullOrWhiteSpace(search))
            {
                return Ok(Posts);
            }
            List<BlogPost> matchingPosts = Posts
            .Where(post =>
            post.Title.Contains(
            search,
            StringComparison.OrdinalIgnoreCase) ||
            post.Content.Contains(
            search,
            StringComparison.OrdinalIgnoreCase))
            .ToList();
            return Ok(matchingPosts);
        }

        //Part 6: Add a GET Route for a Single Post
        [HttpGet("/api/blog/{id:int}")]
        public IActionResult GetById(int id)
        {
            _logger.LogInformation(
            "GET request received for blog post {PostId}.",
            id);
            BlogPost? post = Posts.FirstOrDefault(post => post.Id == id);
            if (post == null)
            {
                _logger.LogWarning(
                "Blog post {PostId} was not found.",
                id);
                return NotFound();
            }
            return Ok(post);
        }


        // Part 8: Add a POST Route
        [HttpPost("/api/blog")]
        public IActionResult CreateAPI([FromBody] BlogPost newPost)
        {
            _logger.LogInformation(
            "POST request received to create a blog post titled {PostTitle}.",
            newPost.Title);
            if (string.IsNullOrWhiteSpace(newPost.Title))
            {
                _logger.LogWarning(
                "A blog post could not be created because its title was empty.");
            return BadRequest("A title is required.");
            }
            int nextId = Posts.Count == 0
            ? 1
            : Posts.Max(post => post.Id) + 1;
            newPost.Id = nextId;
            Posts.Add(newPost);
            return CreatedAtAction(
            nameof(GetById),
            new { id = newPost.Id },
            newPost);
        }

        //Part 9: Add a PUT Route
        [HttpPut("/api/blog/{id:int}")]
        public IActionResult Update(int id, [FromBody] BlogPost updatedPost)
        {
            _logger.LogInformation(
            "PUT request received for blog post {PostId}.",
            id);
            BlogPost? existingPost =
            Posts.FirstOrDefault(post => post.Id == id);
            if (existingPost == null)
            {
                _logger.LogWarning("Blog post {PostId} could not be updated because it was not found.", id);
                return NotFound();
            }
            if (string.IsNullOrWhiteSpace(updatedPost.Title))
            {
                return BadRequest("A title is required.");
            }
            existingPost.Title = updatedPost.Title;
            existingPost.Content = updatedPost.Content;
            return Ok(existingPost);
        }

        //Part 10: Add a DELETE Route
        [HttpDelete("/api/blog/{id:int}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation(
            "DELETE request received for blog post {PostId}.",
            id);
            BlogPost? post =
            Posts.FirstOrDefault(post => post.Id == id);
            if (post == null)
            {
                _logger.LogWarning("Blog post {PostId} could not be deleted because it was not found.",
               
                id);
                return NotFound();
            }
            Posts.Remove(post);
            return NoContent();
        }


        //Week 3 Part 6
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Displaying the Create Blog Post form.");
            BlogPostCreateViewModel viewModel = new();
            return View(viewModel);
        }

        //Week 3 Part 10
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BlogPostCreateViewModel viewModel)
        {
            _logger.LogInformation(
            "Create Blog Post form submitted with title {PostTitle}.",
            viewModel.Title);
            if (!ModelState.IsValid)
            {
                _logger.LogWarning(
                "The Create Blog Post form contained validation errors.");
                return View(viewModel);
            }
            int nextId = Posts.Count == 0
            ? 1
            : Posts.Max(post => post.Id) + 1;
            BlogPost newPost = new()
            {
                Id = nextId,
                Title = viewModel.Title.Trim(),
                Content = viewModel.Content.Trim()
            };
            Posts.Add(newPost);
            _logger.LogInformation(
            "Blog post {PostId} was created successfully.",
            newPost.Id);
            return RedirectToAction(nameof(Index));
        }



        

    }

    
}
