using Microsoft.EntityFrameworkCore;
using railwayapp.Data;

var builder = WebApplication.CreateBuilder(args);

// Read connection string from Railway env var or fallback to appsettings
var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL") ??
    builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseRouting();
app.UseAuthorization();

// Add root endpoint for Railway health check
app.MapGet("/", () => "API Running");

// MVC Default Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Safe Database Migration (no crash on Railway)
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migration Error: " + ex.Message);
    }
}

// Listen on assigned Railway port
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

app.Run();