using ClientPricingSystem.Configuration;
using ClientPricingSystem.Core.Services;
using MongoDB.Bson;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure MongoDB
BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
builder.Services.Configure<DatabaseConfiguration>(builder.Configuration.GetSection("DatabaseConfiguration"));
builder.Services.AddSingleton<IMongoClient, MongoClient>(c => new MongoClient(builder.Configuration.GetConnectionString("DefaultConnectionString")));

// Configure services
builder.Services.AddSingleton<IClientService, ClientService>();
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddSingleton<IVendorService, VendorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
