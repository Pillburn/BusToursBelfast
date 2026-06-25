using AspNetCoreRateLimit;
using ToursApp.Application.Tours.Queries;
using ToursApp.Application;
using ToursApp.Domain.Interfaces;
using ToursApp.Infrastructure;
using ToursApp.Infrastructure.Services;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Infrastructure.Repositories;
using ToursApp.Infrastructure.Persistence;
using ToursApp.API.Endpoints;
using ToursApp.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);// Create webapp builder

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ToursAppInMemory"));

builder.Services.AddScoped<IApplicationDbContext>(provider => 
    provider.GetRequiredService<AppDbContext>());

//Services Add
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
//CORS
// Program.cs

 // Production CORS - specific origins with credentials
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:3000",
                "https://localhost:3000",
                "http://localhost:3001",
                "https://localhost:3001",
                "http://localhost:5173",
                "https://localhost:5173"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        });
});

// Use the appropriate policy
//RATE LIMITING
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
//builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(GetToursQuery).Assembly));

var configuration = builder.Configuration;

//CONFIGURING STRIPE SETTINGS
builder.Services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));

//IN-MEMORY REPO (NO Db)
builder.Services.AddScoped<ITourRepository, InMemoryTourRepository>();
builder.Services.AddScoped<IBookingRepository, InMemoryBookingRepo>();
builder.Services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
builder.Services.AddScoped<IPaymentRepository, InMemoryPaymentRepository>();
//builder.Services.AddScoped<IApplicationDbContext, AppDbContext>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
//builder.Services.AddScoped<IStripeGate, StripeGateway>();

//PAYMENT SERVICES ONLY-IN MEMORY IMP 
// Program.cs
// Use real Stripe
builder.Services.AddScoped<IPaymentGateway, StripePaymentGateway>();
//builder.Services.AddScoped<IPaymentGateway, FakePaymentGateway>();
builder.Services.AddScoped<IStripePaymentService, FakeStripePaymentService>();
builder.Services.AddScoped<IStripeEventMapper, StripeEventMapper>();
builder.Services.AddScoped<IPaymentWebhookService, InMemoryPaymentWebhookService>();

//builder.Services.AddScoped<IPaymentGateway,StripePaymentGateway>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
//builder.Services.AddScoped<IBookingRepository, BookingRepo>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddApplicationServices();
if (!builder.Environment.IsDevelopment())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();

app.UseCors("AllowSpecificOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage(); // ← This is crucial!
}
else
{
    // For production on Render, handle redirection manually if needed.
    // For now, you can comment it out or conditionally disable it.
    // app.UseHttpsRedirection();
    Console.WriteLine("Running in production, HTTPS redirection is disabled.");
}
app.UseIpRateLimiting();
app.UseHttpsRedirection();
//app.UseAuthorization();
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
        {
            error = ex.Message,
            stackTrace = ex.StackTrace
        }));
    }
});
app.MapPost("/api/tour/create-booking", ToursApp.API.Endpoints.TourEndpoints.CreateBooking);
app.MapGet("/api/test", () => Results.Ok(new { message = "API is working!", timestamp = DateTime.UtcNow }));
app.MapGet("/api/tour/first", async (ITourRepository tourRepository, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("Fetching first tour...");
        var tours = await tourRepository.GetAllToursAsync();
        
        logger.LogInformation("Found {Count} tours", tours?.Count() ?? 0);
        if (tours == null || !tours.Any())
        {
            logger.LogWarning("No tours found in database");
            return Results.NotFound(new { message = "No tours found. Please seed some tours first." });
        }
        var firstTour = tours.FirstOrDefault();
        return Results.Ok(new
        {
            id = firstTour!.Id,
            name = firstTour.Title,
            price = firstTour.Price
        });
    }
    catch(Exception ex)
    {
        logger.LogError(ex, "Error fetching first tour {Message}", ex.Message);
        return Results.Problem($"Error fetching tour: {ex.Message}");
    }
});
// Program.cs - Fixed seed endpoint
app.MapGet("/api/tour/seed", async (ITourRepository tourRepository, ILogger<Program> logger) =>
{
    try
    {
        logger.LogInformation("🌱 Checking if tours exist...");
        
        var existingTours = await tourRepository.GetAllToursAsync();
        var existingList = existingTours?.ToList() ?? new List<Tour>();
        
        if (existingList.Any())
        {
            logger.LogInformation("📊 Tours already exist: {Count}", existingList.Count);
            return Results.Ok(new 
            { 
                message = "Tours already exist", 
                count = existingList.Count,
                tours = existingList.Select(t => new { t.Id, t.Title })
            });
        }
        
        logger.LogInformation("🌱 No tours found, seeding...");
        
        var seedTours = new[]
        {
            new Tour(
                description: "Discover the rich history and culture of Belfast...",
                name: "Belfast City Explorer",
                price: 25m,
                groupSize: 7,
                tourDate: DateTime.UtcNow.AddDays(7),
                createdBy: "system"
            )
            {
                Id = Guid.NewGuid(),
                Location = "Belfast",
                IsActive = true,
                MaxCapacity = 20
            },
            new Tour(
                description: "Experience the giants causeway, one of Ireland's most famous landmarks.",
                name: "Causeway Tour",
                price: 20m,
                groupSize: 6,
                tourDate: DateTime.UtcNow.AddDays(14),
                createdBy: "system"
            )
            {
                Id = Guid.NewGuid(),
                Location = "Causeway",
                IsActive = true,
                MaxCapacity = 15
            }
        };
        
        foreach (var tour in seedTours)
        {
            logger.LogInformation("📝 Creating tour: {Title}", tour.Title);
            await tourRepository.CreateTourAsync(tour);
        }
        
        return Results.Ok(new 
        { 
            message = "Seeded tours successfully", 
            count = seedTours.Length,
            tours = seedTours.Select(t => new { t.Id, t.Title })
        });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error seeding tours: {Message}", ex.Message);
        return Results.Problem(
            detail: ex.Message,
            title: "Error seeding tours",
            statusCode: 500
        );
    }
});
// Program.cs - Add this endpoint to seed tours directly
app.MapGet("/api/tour/test", () => Results.Ok(new { 
    message = "Tour API is working!", 
    tours = new[] { new { id = "1", name = "Test Tour" } } 
}));
app.MapGet("/api/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
}));
app.MapTourEndpoints();
app.MapBookingEndpoints();
app.MapControllers();
app.Run();

// This makes the auto-generated 'Program' class visible to the test project.
public partial class Program { }