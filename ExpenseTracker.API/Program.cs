using ExpenseTracker.API.Data;
using Microsoft.EntityFrameworkCore;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Clear default logging providers and add console logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    // ✅ Register DbContext with SQL Server and retry logic
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()
        ));

    // ✅ Configure CORS policy for frontend access
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
        {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // ✅ Add MVC + Swagger/OpenAPI services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // ✅ Apply EF Core migrations at runtime
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

    // ⚠️ Middleware order is important
    app.UseCors(MyAllowSpecificOrigins);   // Place early in pipeline
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
