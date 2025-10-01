
using Business.Interfaces;
using Data.Interfaces;
using Moq;

namespace Test.Business;

public class BookingService_Test
{
    private readonly Mock<IBookingRepository> _repositoryMock;
    private readonly IBookingService _bookingService;
}
