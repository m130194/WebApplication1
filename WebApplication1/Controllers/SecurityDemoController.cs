using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{

    [ApiController]
    [Route("api/security")]
    public sealed class SecurityDemoController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok(new
            {
                message =
            "This endpoint is public."
            });
        }
        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok(new
            {
                username =
            User.Identity?.Name,
                authenticated =
            User.Identity?.IsAuthenticated,
                message =
            "You successfully authenticated."
            });
        }

    }
}
