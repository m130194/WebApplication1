using System;
using System.Collections.Generic;
using System.Text;
using WebApplication1.Controllers;
using Microsoft.AspNetCore.Mvc;

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
    }
}
