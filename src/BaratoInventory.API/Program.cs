using BaratoInventory.Core.Interfaces;
using BaratoInventory.Infrastructure.Data;
using BaratoInventory.Infrastructure.Repositories;
using BaratoInventory.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get SQL connection string (Docker overrides appsettings)
var defaultConnection =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? throw new Exception("SQL connection string is missing.");

// Configure SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(defaultConnection));

// Configure Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
{
    var redisConn =
        builder.Configuration.GetConnectionString("RedisConnection")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__RedisConnection")
        ?? "redis:6379";

    return ConnectionMultiplexer.Connect(redisConn);
});

// Register services
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICacheService, RedisCacheService>();

var app = builder.Build();

// Auto-migrate database on startup with retry logic
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();

        // Apply pending migrations automatically
        context.Database.Migrate();

        Console.WriteLine("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while migrating the database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
