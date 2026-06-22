using System;
using ToursApp.Domain.Entities;

namespace ToursApp.Domain.Interfaces;
public interface ITourRepository
{
    Task<IEnumerable<Tour>> GetAllToursAsync(bool includeInactive = false);
    Task<Tour?> GetTourByIdAsync(Guid id);
    Task<bool> TourExistsAsync(Guid tourId);
}