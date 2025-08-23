using Microsoft.Extensions.DependencyInjection;
using AspNetCoreRateLimit;
using ToursApp.Application.Common.Mappings;
using ToursApp.Application.Tours.Queries;
using ToursApp.Application;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Interfaces;
using ToursApp.Infrastructure;
using ToursApp.Infrastructure.Services;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Infrastructure.Repositories;
using ToursApp.Infrastructure.Services;
using ToursApp.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);// Create webapp builder

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(GetToursQuery).Assembly));
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddOpenApi();
var configuration = builder.Configuration;
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<StripeSettings>(configuration.GetSection("StripeSettings"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddScoped<IApplicationDbContext, AppDbContext>();
builder.Services.AddScoped<IStripeGate, StripeGateway>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
builder.Services.AddScoped<IBookingRepository, BookingRepo>();
builder.Services.AddScoped<ITourRepository, InMemoryTourRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseIpRateLimiting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();

// This makes the auto-generated 'Program' class visible to the test project.
public partial class Program { }