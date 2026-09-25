using System.Net;

namespace WebApplication1.Tests
{
    //Week 10 Part 42: Create Integration Test
    public sealed class SecurityIntegrationTests
 : IClassFixture<BlogAppFactory>
    {
        private readonly HttpClient _client;
        public SecurityIntegrationTests(
        BlogAppFactory factory)
        {
            _client =
            factory.CreateClient();
        }
        
        [Fact]
        public async Task
        PublicEndpoint_ReturnsOk()
        {
            HttpResponseMessage response =
            await _client.GetAsync(
            "/api/security/public");
            Assert.Equal(
            HttpStatusCode.OK,
            response.StatusCode);
        }
        
        [Fact]
        public async Task
        ProtectedEndpoint_WithoutCredentials_ReturnsUnauthorized()
        {
            HttpResponseMessage response =
            await _client.GetAsync(
            "/api/security/profile");
            Assert.Equal(
            HttpStatusCode.Unauthorized,
            response.StatusCode);
        }
    }
}