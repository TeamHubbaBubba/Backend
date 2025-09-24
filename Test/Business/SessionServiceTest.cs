using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace Test.Business;
public class SessionServiceTest
{
    private readonly Mock<ISessionRepository> _sessionsRepositoryMoq = new();
    private readonly ISessionService _sessionService;

    public SessionServiceTest()
    {
        _sessionService = new SessionService(
            _sessionsRepositoryMoq.Object
        );
    }

    // Tests several possible invalid inputs for the session id: nul, empty string, whitespace string
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task GetSessionById_ShouldReturnBadRequest_WhenTheGivenIdIsInvalid(string? testSessionId)
    {
        //Arrange

        //Act
        var result = await _sessionService.GetSessionByIdAsync(testSessionId!);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode); //400 BadRequest
    }

    [Fact]
    public async Task GetSessionById_ShouldReturnNotFound_WhenSessionDoesNotExist()
    {
        //Arrange
        var testSessionId = "nonexistent-session-id";

        _sessionsRepositoryMoq
    .Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>()))
    .ReturnsAsync((SessionEntity?)null);

        //Act
        var result = await _sessionService.GetSessionByIdAsync(testSessionId);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode); //404 NotFound
    }

    [Fact]
    public async Task GetSessionById_ShouldReturnSession_WhenSessionExists()
    {
        //Arrange
        var testSessionId = "testingId";
        var testSessionEntity = new SessionEntity
        {
            Id = testSessionId,
            Title = "Test Session",
            Description = "Test Description",
            MaxParticipants = 10,
            CurrentParticipants = 0,
            Date = DateTime.Now
        };

        var testSessionModel = new SessionModel
        {
            Id = testSessionId,
            Title = "Test Session",
            Description = "Test Description",
            MaxParticipants = 10,
            CurrentParticipants = 0,
            Date = DateTime.Now
        };

        _sessionsRepositoryMoq
            .Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>()))
            .ReturnsAsync(testSessionEntity);

        //Act
        var result = await _sessionService.GetSessionByIdAsync(testSessionId);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        var returnedSessionModel = Assert.IsType<ResponseResult<SessionModel>>(result);
        Assert.Equal(testSessionModel, returnedSessionModel.Data);
        Assert.Equal(200, result.StatusCode); //200 OK
    }
}
