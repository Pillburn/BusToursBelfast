namespace ToursApp.Domain.Entities;


public class Tour
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int GroupSize { get; private set; }
    public string Location { get; set; } = string.Empty;
    public DateTime BookingDate { get; private set; }
    public DateTime TourDate { get; init; } 
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    // Private constructor for EF Core
    public Tour() { }

    public Tour(string name, string description, decimal price, int groupSize, DateTime tourDate)
    {
        Id = Guid.NewGuid();
        Title = name;
        Description = description;
        Price = price;
        GroupSize = groupSize;
        TourDate = tourDate;
        Validate();
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