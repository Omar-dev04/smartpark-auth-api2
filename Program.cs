using Microsoft.EntityFrameworkCore;
using railwayapp.Data; // your namespace

var builder = WebApplication.CreateBuilder(args);

// Read connection string from appsettings OR Railway env vars
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register EF Core with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Auto-apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
