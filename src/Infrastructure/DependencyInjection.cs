using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ToursApp.Domain.Interfaces;
using ToursApp.Application.Common.Interfaces;
using ToursApp.Infrastructure.Services;
using ToursApp.Infrastructure.Repositories;
using ToursApp.Infrastructure.Persistence;

namespace ToursApp.Infrastructure;  // Same namespace as the using directive

public static class DependencyInjection  // Must be public
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Register IApplicationDbContext to use AppDbContext
        services.AddScoped<IApplicationDbContext>(provider => 
            provider.GetRequiredService<AppDbContext>());
        
        // Register CurrentUserService
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        // Register repositories
        services.AddScoped<IBookingRepository, BookingRepo>();
        
        return services;
    }
}