using Business.Interfaces;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace Test.Business;

public class SessionService_Tests
{
    private readonly Mock<ISessionRepository> _repositoryMock;
    private readonly ISessionService _sessionService;

    public SessionService_Tests()
    {
        _repositoryMock = new Mock<ISessionRepository>();
        _sessionService = new SessionService(_repositoryMock.Object);
    }

    [Fact]
    public async Task DeleteSessionAsync_ShouldReturnOk_WhenRepoReturnsTrue()
    {
        // Arrange
        Expression<Func<SessionEntity, bool>> expression = x => x.Id == Guid.NewGuid().ToString();
        _repositoryMock
            .Setup(x => x.DeleteAsync(expression))
            .ReturnsAsync(true);

        // Act
        var result = await _sessionService.DeleteSessionAsync(expression);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task DeleteSessionAsync_ShouldReturnNotFound_WhenRepoReturnsFalse()
    {
        // Arrange
        Expression<Func<SessionEntity, bool>> expression = x => x.Id == Guid.NewGuid().ToString();
        _repositoryMock
            .Setup(x => x.DeleteAsync(expression))
            .ReturnsAsync(false);

        // Act
        var result = await _sessionService.DeleteSessionAsync(expression);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
        Assert.Equal("Session was not found", result.ResultMessage);
    }

    [Fact]
    public async Task DeleteSessionAsync_ShouldReturnError_WhenRepoThrowsEx()
    {
        // Arrange
        Expression<Func<SessionEntity, bool>> expression = x => x.Id == Guid.NewGuid().ToString();
        _repositoryMock
            .Setup(x => x.DeleteAsync(expression))
            .ThrowsAsync(new Exception("Internal error"));

        // Act
        var result = await _sessionService.DeleteSessionAsync(expression);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Equal("Error deleting session", result.ResultMessage);
    }

    [Fact]
    public async Task DeleteSessionAsync_ShouldReturnBadRequest_WhenExpressionIsNull()
    {
        // Act
        var result = await _sessionService.DeleteSessionAsync(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Invalid request", result.ResultMessage);
    }
}

