using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using WebApplication1.Controllers;

namespace WebApplication1.Tests
{
    public sealed class SecurityDemoControllerTests
    {
        [Fact]
        public void Public_ReturnsOkResult()
        {
            // Arrange
            SecurityDemoController controller =
            new();
            // Act
            IActionResult result =
            controller.Public();
            // Assert
            OkObjectResult okResult =
            Assert.IsType<OkObjectResult>(
            result);
            Assert.Equal(
            200,
            okResult.StatusCode);
        }

        [Fact]
        public void Profile_WithAuthenticatedUser_ReturnsOk()
        {
            // Arrange
            SecurityDemoController controller =
            new();
            ClaimsIdentity identity =
            new(
            [
            new Claim(
 ClaimTypes.Name,
"editor")
            ],
            "TestAuthentication");
            ClaimsPrincipal user =
            new(identity);
            controller.ControllerContext =
            new ControllerContext
            {
                HttpContext =
            new DefaultHttpContext
            {
                User = user
            }
            };
            // Act
            IActionResult result =
            controller.Profile();
            // Assert
            Assert.IsType<OkObjectResult>(
            result);
            Assert.True(
            controller.User.Identity?
            .IsAuthenticated);
            Assert.Equal(
            "editor",
            controller.User.Identity?.Name);
        }

    }
}
