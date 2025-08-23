// tests/Domain.UnitTests/Entities/TourTests.cs
using NUnit.Framework;
using ToursApp.Domain.Entities;  // Fixed namespace

namespace Domain.UnitTests.Entities;

[TestFixture]
public class TourTests
{
    [Test]
    public void Tour_Should_Have_Valid_Duration()
    {
        // Arrange & Act
        var tour = new Tour(
            "Test Tour", 
            "Test Description", 
            100.00m, 
            5, 
            DateTime.UtcNow
        );
        
        // Assert
        Assert.That(tour.DurationDays, Is.GreaterThan(0));
    }

    [Test]
    public void Simple_Test_To_Verify_Setup()
    {
        Assert.That(1 + 1, Is.EqualTo(2));
    }
}