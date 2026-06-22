

using ToursApp.Domain.Entities;

namespace ToursApp.Application.Tours.DTOs;

public class TourDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Title { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = " ";
    public required string Location { get; set; } //this will only really matter for the build your own or executive tours because otherwise locations are set
    public required double Price { get; set; }
    public required int GroupSize { get; set; } // this will be set by bus size really\
    public DateTime BookingDate { get; private set; }
    public DateTime TourDate { get; init; } 
    public bool IsActive { get; set; }
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>(); 

}