using WebApplication1.Configuration;
using WebApplication1.Services;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

//Register MongoDB in Program.cs
//MongoClient manages the connection to the MongoDB deployment
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
MongoDbSettings mongoDbSettings = new()
{
    ConnectionString = connectionString,
    DatabaseName = databaseName,
    CollectionName = collectionName
};
builder.Services.AddSingleton(mongoDbSettings);
builder.Services.AddSingleton<IMongoClient>(
 new MongoClient(mongoDbSettings.ConnectionString));
builder.Services.AddSingleton<MongoBlogPostService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//required for the attribute-routed API controller
app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
