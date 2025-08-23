using System;
using ToursApp.Domain.Entities;

namespace ToursApp.Domain.Interfaces;
public interface ITourRepository
{
    Task<List<Tour>> GetAllToursAsync();
    Task<IEnumerable<Tour>> GetAllToursAsync(bool includeInactive = false);
    Task<Tour?> GetTourByIdAsync(string id);
}