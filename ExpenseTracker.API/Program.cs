using ExpenseTracker.API.Data;
using Microsoft.EntityFrameworkCore;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // ✅ Logging
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    // ✅ Register DbContext with Azure SQL + retry
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.EnableRetryOnFailure()
        ));

    // ✅ Configure CORS for frontend access (localhost + Netlify)
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "https://fintrackexp.netlify.app"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // ✅ Add API + Swagger services
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // ✅ Apply EF Core migrations automatically
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✅ EF Core migrations applied (if any).");
    }

    // ✅ Always enable Swagger (even in production)
    app.UseSwagger();
    app.UseSwaggerUI();

    // ✅ Middleware pipeline
    app.UseCors(MyAllowSpecificOrigins);
    app.UseHttpsRedirection();
    app.UseAuthorization();

    // ✅ Map API controllers
    app.MapControllers();

    // ✅ Map a root route to show status
    app.MapGet("/", () => "✅ FinTrack Backend API is running!");

    Console.WriteLine("✅ Starting ExpenseTracker API...");
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("❌ Application failed to start.");
    Console.WriteLine($"Error: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");
}
