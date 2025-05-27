using ExpenseTracker.API.Data;
using Microsoft.EntityFrameworkCore;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    // Register DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // CORS Policy
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
        {
            policy.WithOrigins("http://localhost:5173") // React frontend dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // Controllers and Swagger
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // ✅ Run EF Core migrations on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✅ EF Core migrations applied (if any).");
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // ⚠️ CORS should be placed early (before Authorization or routing)
    app.UseCors(MyAllowSpecificOrigins);

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Console.WriteLine("✅ Starting ExpenseTracker API...");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("❌ Application failed to start.");
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");
}
