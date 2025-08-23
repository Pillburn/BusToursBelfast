// tests/Application.UnitTests/Features/Payments/CreatePaymentIntentTests.cs
using NUnit.Framework;
using Application.Features.Payments.Commands.CreatePaymentIntent;

namespace Application.UnitTests.Features.Payments;

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