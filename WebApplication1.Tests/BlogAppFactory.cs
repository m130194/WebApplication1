using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;


namespace WebApplication1.Tests
{

    //This is an integration test
    //starts your app in a test environment
    //without requiring app to be manually started from Visual Studio, this allows tests to send HTTP requests through:
    //ASP.NET Core routing
    //Middleware
    //Authentication
    //Authorization
    //Controllers

    public sealed class BlogAppFactory
 : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(
        IWebHostBuilder builder)
        {
            builder.UseEnvironment(
            "Testing");
            builder.ConfigureAppConfiguration(
            (context, configuration) =>
            {
                Dictionary<string, string?>
     settings =
    new()
    {
        [
     "MongoDb:ConnectionString"
     ] =
     "mongodb://localhost:27017",
        [
     "MongoDb:DatabaseName"
     ] =
     "BlogDb",
        [
     "MongoDb:CollectionName"
     ] =
     "BlogPosts",
        [
     "MongoDb:ActivityLogCollectionName"
     ] =
     "ActivityLogs",
        [
     "BasicAuth:AdminUsername"
     ] =
     "admin",
        [
     "BasicAuth:AdminPassword"
     ] =
     "TestAdminPassword",
        [
     "BasicAuth:EditorUsername"
     ] =
     "editor",
        [
     "BasicAuth:EditorPassword"
     ] =
     "TestEditorPassword"
    };
                configuration
     .AddInMemoryCollection(
     settings);
            });
        }
    }
}