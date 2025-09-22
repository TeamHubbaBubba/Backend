using System.Threading.Tasks;
using Xunit;
using Moq;
using Business.Services;
using Business.Interfaces;
using Business.Models;
using Data.Interfaces;
using Data.Entities;
using Business.Dtos;

public class SessionServiceTests
{
    // Enkel sub så att vi kan injicera IResponseResult (ResponseResult är abstrakt). \\ Help from chat-gpt //
    private class TestResponseResult : ResponseResult { }

    private readonly Mock<ISessionRepository> _repoMock = new();
    private readonly IResponseResult _responseResult = new TestResponseResult();

    private SessionService CreateService() => new(_repoMock.Object, _responseResult);

    [Fact]
    public async Task CreateSessionAsync_WhenFormIsNull_ReturnsBadRequest()
    {
        // Arrange
        var sut = CreateService();

        // Act
        var result = await sut.CreateSessionAsync(null!);

        // Assert
        var rr = Assert.IsType<ErrorResult>(result);       // BadRequest returnerar ErrorResult
        Assert.False(rr.Success);
        Assert.Equal(400, rr.StatusCode);
        Assert.Equal("Invalid form", rr.ResultMessage);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<SessionEntity>()), Times.Never);
    }

    [Fact]
    public async Task CreateSessionAsync_WhenRepoReturnsNull_ReturnsBadRequest()
    {
        // Arrange
        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<SessionEntity>()))
            .ReturnsAsync((SessionEntity)null!);

        var sut = CreateService();

        // Act
        var result = await sut.CreateSessionAsync(new SessionDto()); // innehållet kvittar i detta test

        // Assert
        var rr = Assert.IsType<ErrorResult>(result);
        Assert.False(rr.Success);
        Assert.Equal(400, rr.StatusCode);
        Assert.Equal("Enter all required fields.", rr.ResultMessage);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<SessionEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateSessionAsync_WhenRepoReturnsEntity_ReturnsOkWithData()
    {
        // Arrange
        var created = new SessionEntity { /* sätt ev. egenskaper */ };
        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<SessionEntity>()))
            .ReturnsAsync(created);

        var sut = CreateService();

        // Act
        var result = await sut.CreateSessionAsync(new SessionDto());

        // Assert
        var rr = Assert.IsType<ResponseResult<SessionEntity>>(result);
        Assert.True(rr.Success);
        Assert.Equal(200, rr.StatusCode);
        Assert.Same(created, rr.Data);

        _repoMock.Verify(r => r.CreateAsync(It.IsAny<SessionEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateSessionAsync_WhenRepoThrows_ReturnsError()
    {
        // Arrange
        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<SessionEntity>()))
            .ThrowsAsync(new System.Exception("db is down"));

        var sut = CreateService();

        // Act
        var result = await sut.CreateSessionAsync(new SessionDto());

        // Assert
        var rr = Assert.IsType<ErrorResult>(result);
        Assert.False(rr.Success);
        Assert.Equal(500, rr.StatusCode);
        Assert.Equal("Failed to create session", rr.ResultMessage);
    }
}
