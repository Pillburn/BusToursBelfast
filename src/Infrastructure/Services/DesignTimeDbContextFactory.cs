// In MySolution.Persistence/DesignTimeDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ToursApp.Infrastructure.Persistence;

// This class is needed for 'dotnet ef' commands to work
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        // This connection string is only used for migrations and scaffolding
        optionsBuilder.UseNpgsql("Host=localhost;Database=mydotnetdb;Username=myuser;Password=password123");

        return new AppDbContext(optionsBuilder.Options);
    }
}