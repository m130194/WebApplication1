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

        //Week 9 Part 27 Add Role-Based Authorization
        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok(new
            {
                username =
            User.Identity?.Name,
                message =
            "You have the Admin role."
            });
        }

    }
}
