using System;
using ToursApp.Domain.Entities;

namespace ToursApp.Domain.Interfaces;

public interface ITourRepository
{
    Task<Tour?> GetByIdAsync(Guid id);
    Task<IEnumerable<Tour>> GetAllAsync(bool includeInactive = false);
    Task AddAsync(Tour tour);
    Task UpdateAsync(Tour tour);
    Task DeleteAsync(Guid id);
}

