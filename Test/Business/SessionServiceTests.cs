using System;
using System.Threading.Tasks;
using Business.Dtos;
using Business.Models;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Moq;
using Xunit;

namespace Test.Business;

public class SessionServiceTests
{
    [Fact]
    public async Task CreateSessionAsync_NullForm_ReturnsBadRequest()
    {
        // Arrange
        var repoMock = new Mock<ISessionRepository>();
        var service = new SessionService(repoMock.Object);

        // Act
        var result = await service.CreateSessionAsync(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Invalid form", result.ResultMessage);
        repoMock.Verify(r => r.CreateAsync(It.IsAny<SessionEntity>()), Times.Never);
    }

    [Fact]
    public async Task CreateSessionAsync_RepositoryReturnsNull_ReturnsBadRequestWithMessage()
    {
        // Arrange
        var repoMock = new Mock<ISessionRepository>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<SessionEntity>()))!
                .ReturnsAsync((SessionEntity?)null);

        var dto = new SessionDto
        {
            Title = "T",
            Description = "D",
            MaxParticipants = 5,
            CurrentParticipants = 0,
            Date = DateTime.UtcNow
        };

        var service = new SessionService(repoMock.Object);

        // Act
        var result = await service.CreateSessionAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Enter all required fields.", result.ResultMessage);
        repoMock.Verify(r => r.CreateAsync(It.IsAny<SessionEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateSessionAsync_ValidDto_RepositoryReturnsEntity_ReturnsOkWithData()
    {
        // Arrange
        var repoMock = new Mock<ISessionRepository>();

        var dto = new SessionDto
        {
            Title = "My Title",
            Description = "My Description",
            MaxParticipants = 10,
            CurrentParticipants = 1,
            Date = new DateTime(2025, 9, 23)
        };

        // Simulate repository returning the created entity (usually the same or with Id populated)
        var createdEntity = new SessionEntity
        {
            Title = dto.Title,
            Description = dto.Description,
            MaxParticipants = dto.MaxParticipants,
            CurrentParticipants = dto.CurrentParticipants,
            Date = dto.Date
        };

        repoMock.Setup(r => r.CreateAsync(It.Is<SessionEntity>(s =>
                s.Title == dto.Title &&
                s.Description == dto.Description &&
                s.MaxParticipants == dto.MaxParticipants &&
                s.CurrentParticipants == dto.CurrentParticipants &&
                s.Date == dto.Date)))
            .ReturnsAsync(createdEntity);

        var service = new SessionService(repoMock.Object);

        // Act
        var result = await service.CreateSessionAsync(dto);

        // Assert
        var typed = Assert.IsType<ResponseResult<SessionEntity>>(result);
        Assert.True(typed.Success);
        Assert.Equal(200, typed.StatusCode);
        Assert.NotNull(typed.Data);
        Assert.Same(createdEntity, typed.Data);
        repoMock.Verify(r => r.CreateAsync(It.IsAny<SessionEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateSessionAsync_RepositoryThrows_ReturnsErrorResult()
    {
        // Arrange
        var repoMock = new Mock<ISessionRepository>();
        repoMock.Setup(r => r.CreateAsync(It.IsAny<SessionEntity>()))
                .ThrowsAsync(new Exception("db failure"));

        var dto = new SessionDto
        {
            Title = "T",
            Description = "D",
            MaxParticipants = 3,
            CurrentParticipants = 0,
            Date = DateTime.UtcNow
        };

        var service = new SessionService(repoMock.Object);

        // Act
        var result = await service.CreateSessionAsync(dto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Equal("Failed to create session", result.ResultMessage);
        repoMock.Verify(r => r.CreateAsync(It.IsAny<SessionEntity>()), Times.Once);
    }
}