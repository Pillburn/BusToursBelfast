// Tests/Application/Bookings/CreateBookingCommandHandlerTests.cs
using Moq;
using Xunit;

public class CreateBookingCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidRequest_ReturnsPaymentResult()
    {
        // Arrange
        var mockBookingRepo = new Mock<IBookingRepository>();
        var mockTourRepo = new Mock<ITourRepository>();
        var mockPaymentGateway = new Mock<IPaymentGateway>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockLogger = new Mock<ILogger<CreateBookingCommandHandler>>();

        var handler = new CreateBookingCommandHandler(
            mockBookingRepo.Object,
            mockTourRepo.Object,
            mockUnitOfWork.Object,
            mockPaymentGateway.Object,
            mockLogger.Object
        );

        var command = new CreateBookingCommand
        {
            TourId = Guid.NewGuid(),
            TourName = "Test Tour",
            TourPrice = 100,
            FullName = "John Doe",
            Email = "john@example.com",
            // ... other properties
        };

        var tour = new Tour("system") { Id = command.TourId, Name = "Test Tour" };
        mockTourRepo.Setup(x => x.GetTourByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(tour);

        mockPaymentGateway.Setup(x => x.CreatePaymentIntentAsync(
            It.IsAny<decimal>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, string>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentIntentResult
            {
                Success = true,
                PaymentIntentId = "pi_test_123",
                ClientSecret = "test_secret_123"
            });

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.ClientSecret);
        Assert.Equal("pi_test_123", result.PaymentIntentId);
        mockBookingRepo.Verify(x => x.CreateBookingAsync(It.IsAny<Booking>()), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task Handle_TourNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var mockTourRepo = new Mock<ITourRepository>();
        mockTourRepo.Setup(x => x.GetTourByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Tour)null);

        var handler = new CreateBookingCommandHandler(
            new Mock<IBookingRepository>().Object,
            mockTourRepo.Object,
            new Mock<IUnitOfWork>().Object,
            new Mock<IPaymentGateway>().Object,
            new Mock<ILogger<CreateBookingCommandHandler>>().Object
        );

        var command = new CreateBookingCommand { TourId = Guid.NewGuid() };

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}