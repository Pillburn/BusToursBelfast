namespace ToursApp.Domain.Entities;


public class Tour
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int GroupSize { get; private set; }
    public DateTime BookingDate { get; private set; }
    public DateTime TourDate { get; init; } 
    public bool IsActive { get; private set; } = true;

    // Private constructor for EF Core
    private Tour() { }

    public Tour(string name, string description, decimal price, int groupSize, DateTime tourDate)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Price = price;
        GroupSize = groupSize;
        TourDate = tourDate;
        Validate();
    }

    public void Update(string name, string description, decimal price)
    {
        Name = name;
        Description = description;
        Price = price;
        Validate();
    }

    public void Activate() => IsActive = true;
    public void Deactivate() => IsActive = false;

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Tour name cannot be empty");
        
        if (Price <= 0)
            throw new ArgumentException("Price must be positive");
    }
}