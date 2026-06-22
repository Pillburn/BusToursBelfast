// Infrastructure/Repositories/TourRepo.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ToursApp.Domain.Entities;
using ToursApp.Domain.Interfaces;
using ToursApp.Infrastructure.Data;

namespace ToursApp.Infrastructure.Repositories
{
    public class TourRepo : ITourRepository
    {
        private readonly ApplicationDbContext _context;

        public TourRepo(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Tour>> GetAllToursAsync()
        {
            return await _context.Tours
                .Include(t => t.Bookings) // Include related bookings if needed
                .Where(t => t.IsActive)
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

        public async Task<Tour?> GetTourByIdAsync(Guid id)
        {
            return await _context.Tours
                .Include(t => t.Bookings)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tour?> GetTourByNameAsync(string name)
        {
            return await _context.Tours
                .FirstOrDefaultAsync(t => t.Title == name);
        }

        /**public async Task<IEnumerable<Tour>> GetToursByLocationAsync(string location)
        {
            return await _context.Tours
                .Where(t => t. != null && t.Location.Contains(location))
                .ToListAsync();
        }**/

        public async Task<IEnumerable<Tour>> GetAvailableToursAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Tours
                .Where(t => t.IsActive &&
                           t.TourDate >= startDate &&
                           t.BookingDate <= endDate)
                .ToListAsync();
        }

        public async  Task<IEnumerable<Tour>> GetAllToursAsync(bool includeInactive = false)
        {
            var query = _context.Tours
                .Include(t => t.Bookings) // Include related bookings if needed
                .OrderBy(t => t.Title)
                .AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(t => t.IsActive);
                }

                return await query.ToListAsync();
        }

        public async Task CreateTourAsync(Tour tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            await _context.Tours.AddAsync(tour);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTourAsync(Tour tour)
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            _context.Tours.Update(tour);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTourAsync(Guid id)
        {
            var tour = await GetTourByIdAsync(id);
            if (tour != null)
            {
                // Soft delete - just mark as inactive
                tour.IsActive = false;
                await UpdateTourAsync(tour);
                
                // Or hard delete:
                // _context.Tours.Remove(tour);
                // await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TourExistsAsync(Guid id)
        {
            return await _context.Tours.AnyAsync(t => t.Id == id);
        }

        public async Task<bool> TourExistsByNameAsync(string name)
        {
            return await _context.Tours.AnyAsync(t => t.Title == name);
        }
    }
}