using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToursApp.Infrastructure.Persistence;

namespace ToursApp.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Register other infrastructure services
        services.AddScoped<ITourRepository, TourRepository>();
        
        return services;
    }
}