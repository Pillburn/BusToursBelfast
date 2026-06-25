using ToursApp.Domain.Common;

namespace ToursApp.Domain.Entities;


public class Tour: BaseEntity
{
    
    // Add a parameterless constructor for EF Core
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int GroupSize { get; set; }
    public string Location { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime TourDate { get; init; } 
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public int MaxCapacity { get; set; }

    // Private constructor for EF Core
    public Tour() 
            : base("system")
    {
        Id = Guid.NewGuid();
        IsActive = true;
    }
    public Tour(string name, string description, decimal price, int groupSize, DateTime tourDate, string createdBy)
            : base("system")
    {
        Id = Guid.NewGuid();
        Title = name;
        Description = description;
        Price = price;
        GroupSize = groupSize;
        TourDate = tourDate;
        Validate();
    }

    public Tour(string createdBy) : base(createdBy)
    {
    }

    public void Update(string name, string description, decimal price)
    {
        Title = name;
        Description = description;
        Price = price;
        Validate();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new ArgumentException("Tour name cannot be empty");
        
        if (Price <= 0)
            throw new ArgumentException("Price must be positive");
    }
}