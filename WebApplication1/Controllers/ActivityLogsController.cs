using WebApplication1.Services;
using WebApplication1.ViewModels;
using Microsoft.AspNetCore.Mvc;
//required for authorize
using Microsoft.AspNetCore.Authorization;

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

        //Week 9 Part 37 Protect an Administrative Operation to create database indexes
        [Authorize(Roles = "Admin")]
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

        //Week 9 Part 36 Protect a real application route
        [Authorize]
        //Week 8 Part 38 Add the GET Endpoint
        [HttpGet]
        public async Task<
         ActionResult<
         List<ActivityLogSummaryViewModel>>>
         GetRecent(
         [FromQuery] int limit = 20,
         CancellationToken cancellationToken = default)
        {
            limit =
            Math.Clamp(
            limit,
            1,
            100);
            List<ActivityLogSummaryViewModel> logs =
            await _service.GetRecentAsync(
            limit,
            cancellationToken);
            return Ok(logs);
        }

        


    }
}
