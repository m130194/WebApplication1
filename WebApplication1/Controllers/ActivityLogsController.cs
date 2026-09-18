using WebApplication1.Services;
using WebApplication1.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/activity-logs")]
    public sealed class ActivityLogsController
 : ControllerBase
    {
        private readonly MongoActivityLogService
        _service;
        public ActivityLogsController(
        MongoActivityLogService service)
        {
            _service =
            service;
        }
        [HttpPost("indexes")]
        public async Task<IActionResult> CreateIndexes(
        CancellationToken cancellationToken)
        {
            await _service.CreateIndexesAsync(
            cancellationToken);
            return Ok(new
            {
                message =
            "Activity log indexes created."
            });
        }
    }
}
