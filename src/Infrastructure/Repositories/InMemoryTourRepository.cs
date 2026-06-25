using ToursApp.Domain.Interfaces;
using ToursApp.Domain.Entities;

public class InMemoryTourRepository : ITourRepository
{
    private readonly List<Tour> _tours = new();

    public InMemoryTourRepository()
    {
        SeedData();
    }

    public void SeedData()
    {
        _tours.AddRange(new[]
        {
        
            new Tour(
                name: "Belfast Political History Tour",
                description: "Explore the history of The Troubles and peace walls with a local guide.",
                price: 25.00m,
                groupSize: 15,
                tourDate: DateTime.Today.AddDays(3),
                createdBy: "system"
            )
            {
                Id = Guid.Parse("95c3eff0-4cb3-49aa-9f14-b432f203eeaa"),
                Location = "Belfast",
                IsActive = true 
            },
            new Tour(
                name: "Giants Causeway Day Trip",
                description: "Journey along the Causeway Coast to the UNESCO World Heritage Site.",
                price: 45.00m,
                groupSize: 20,
                tourDate: DateTime.Today.AddDays(5),
                createdBy: "system"
            )
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                Location = "Causeway",
                IsActive = true 
            },
        });
    }

    public async Task<IEnumerable<Tour>> GetAllToursAsync(bool includeInactive = false)
    {
        var tours = _tours.AsEnumerable();

        if (!includeInactive)
        {
            tours = tours.Where(t => t.IsActive);
        }

        return await Task.FromResult(tours);
    }

    public Task<Tour?> GetTourByIdAsync(string id)
    {
        // Your entity uses Guid, so the ID is a Guid, not a string.
        // We need to parse the string argument to a Guid for comparison.
        if (!Guid.TryParse(id, out var tourId))
        {
            return Task.FromResult((Tour?)null);
        }

        var tour = _tours.FirstOrDefault(t => t.Id == tourId);
        return Task.FromResult(tour);
    }

    // Your interface likely requires a GetByIdAsync that takes a Guid as well.
    // This is a more efficient overload.
    public Task<Tour?> GetTourByIdAsync(Guid id)
    {
        var tour = _tours.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(tour);
    }
    
    public Task<List<Tour>> GetAllToursAsync()
    {
        // Simply return a copy of the _tours list to avoid external modifications
        // to the internal list
        return Task.FromResult(_tours.ToList());
    }

    public async Task<bool> TourExistsAsync(Guid id)
    {
        return await Task.FromResult(_tours.Any(t => t.Id == id));
    }

    public async Task<bool> TourExistsByNameAsync(string name)
    {
        return await Task.FromResult(_tours.Any(t => t.Title == name));
    }

    public Task CreateTourAsync(Tour tour)
    {
        throw new NotImplementedException();
    }
}