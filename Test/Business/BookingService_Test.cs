
using Business.Interfaces;
using Business.Services;
using Data.Interfaces;
using Moq;

namespace Test.Business;

public class BookingService_Test
{
    private readonly Mock<IBookingRepository> _repositoryMock;
    private readonly IBookingService _bookingService;
    public BookingService_Test()
    {
        _repositoryMock = new Mock<IBookingRepository>();
        _bookingService = new BookingService(_repositoryMock.Object);
    }

    //Create

    //Create should return bad request when sessionId is null
    [Fact]
    public async Task CreateBooking_ShouldReturnBadRequest_WhenSessionIdIsNull()
    {
        //Arrange
        string sessionId = null;
        Guid userId = Guid.NewGuid();

        // Act
        var result = await _bookingService.CreateBookingAsync(sessionId!, userId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);

    }
    //Create should return error when repository returns null
    [Fact]
    public async Task CreateBooking_ShouldReturnError_WhenRepoReturnsNull()
    {
        //Arrange
        string sessionId = Guid.NewGuid().ToString();
        Guid userId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Data.Entities.BookingEntity >()))
            .ReturnsAsync((Data.Entities.BookingEntity?)null);

        // Act

        var result = await _bookingService.CreateBookingAsync(sessionId, userId);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);

    }
    //Create should return ok when repository returns a booking
    [Fact]
    public async Task CreateBooking_ShouldReturnOk_WhenRepoReturnsBooking()
    {
        //Arrange
        string sessionId = Guid.NewGuid().ToString();
        string bookingId = Guid.NewGuid().ToString();
        Guid userId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Data.Entities.BookingEntity>()))
            .ReturnsAsync(new Data.Entities.BookingEntity { Id = bookingId, SessionId = sessionId, UserId = userId });
        // Act
        var result = await _bookingService.CreateBookingAsync(sessionId, userId);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);

    }
    //Create should return error when exception is thrown
    [Fact]
    public async Task CreateBooking_ShouldReturnError_WhenAnExceptionisThrown()
    {
        //Arrange
        string sessionId = Guid.NewGuid().ToString();
        Guid userId = Guid.NewGuid();
        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Data.Entities.BookingEntity>()))
            .ThrowsAsync(new Exception());
        // Act
        var result = await _bookingService.CreateBookingAsync(sessionId, userId);
        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);

    }

    //Read
    //Update
    //Delete
}
