// tests/Application.UnitTests/Features/Payments/CreatePaymentIntentTests.cs
namespace Application.UnitTests.Features.Payments;
using  ToursApp.Application.Payments.Commands;
[TestFixture]
public class CreatePaymentIntentTests
{
    [Test]
    public void Test1()
    {
        // Arrange
        var command = new CreatePaymentIntentCommand(100.00m, "usd");
        
        // Act & Assert
        Assert.That(command.Amount, Is.EqualTo(100.00m));
    }
}