using Microsoft.EntityFrameworkCore;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Interfaces;
using ToursApp.Infrastructure.Persistence;

namespace ToursApp.Infrastructure.Repositories;

public class TourRepository : ITourRepository
{
    private readonly AppDbContext _context;

    public TourRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tour?> GetByIdAsync(Guid id)
    {
        return await _context.Tours.FindAsync(id);
    }

    public async Task<IEnumerable<Tour>> GetAllAsync(bool includeInactive = false)
    {
        return await _context.Tours
            .Where(t => includeInactive || t.IsActive)
            .ToListAsync();
    }

    public async Task AddAsync(Tour tour)
    {
        await _context.Tours.AddAsync(tour);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Tour tour)
    {
        _context.Tours.Update(tour);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var tour = await GetByIdAsync(id);
        if (tour != null)
        {
            _context.Tours.Remove(tour);
            await _context.SaveChangesAsync();
        }
    }
}