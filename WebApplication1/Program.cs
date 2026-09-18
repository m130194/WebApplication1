using Microsoft.AspNetCore.Authentication;
using MongoDB.Driver;
using System.Net;
using WebApplication1.Configuration;
using WebApplication1.Security;
using WebApplication1.Services;

var builder = WebApplication.CreateBuilder(args);

//Register MongoDB in Program.cs
//MongoClient manages the connection to the MongoDB deployment
//Configuration in MongoDbSettings.cs
string connectionString =
 builder.Configuration["MongoDb:ConnectionString"]
 ?? throw new InvalidOperationException(
 "MongoDb:ConnectionString has not been configured.");
string databaseName =
 builder.Configuration["MongoDb:DatabaseName"]
 ?? throw new InvalidOperationException(
 "MongoDb:DatabaseName has not been configured.");
string collectionName =
 builder.Configuration["MongoDb:CollectionName"]
 ?? throw new InvalidOperationException(
 "MongoDb:CollectionName has not been configured.");
string activityLogCollectionName =
 builder.Configuration["MongoDb:ActivityLogCollectionName"]
 ?? throw new InvalidOperationException(
 "MongoDb:ActivityLogCollectionName has not been configured.");
MongoDbSettings mongoDbSettings = new()
{
    ConnectionString = connectionString,
    DatabaseName = databaseName,
    CollectionName = collectionName,
    ActivityLogCollectionName = activityLogCollectionName
};
builder.Services.AddSingleton(mongoDbSettings);
builder.Services.AddSingleton<IMongoClient>(
 new MongoClient(mongoDbSettings.ConnectionString));
builder.Services.AddSingleton<MongoBlogPostService>();
//Week 8 Part 22 Register the Activity Log Service
builder.Services.AddSingleton<MongoActivityLogService>();
//Week 8 Part 23 Register the Background Service that starts when the ASP.NET Core application starts
builder.Services.AddHostedService<BlogPostChangeWatcher>();


// Add services to the container.
builder.Services.AddControllersWithViews();


//Week 9 Part 17 Register Basic Authentication
builder.Services
 .AddAuthentication(options =>
 {
     options.DefaultAuthenticateScheme =
     "Basic";
     options.DefaultChallengeScheme =
     "Basic";
 })
 .AddScheme<
 AuthenticationSchemeOptions,
 BasicAuthenticationHandler>(
 "Basic",
 options => { });

//Week 9 Part 18 Add Authorization
//builder.Services.AddAuthorization();

//Week 9 Part 31 Create a Policy-Based Authorization Rule
builder.Services.AddAuthorization(
 options =>
 {
     options.AddPolicy(
     "CanManageBlog",
     policy =>
     {
         policy.RequireClaim(
     "Permission",
    "ManageBlog");
     });
 });





var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//middleware section

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

//required for the attribute-routed API controller
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
