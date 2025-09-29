using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Moq;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            Date = new DateTime(2025, 9, 23, 21, 45, 32, 123)
        };

        var testSessionModel = new SessionModel
        {
            Id = testSessionId,
            Title = "Test Session",
            Description = "Test Description",
            MaxParticipants = 10,
            CurrentParticipants = 0,
            Date = new DateTime(2025, 9, 23, 21, 45, 32, 123)
        };

        _repositoryMock
            .Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>()))
            .ReturnsAsync(testSessionEntity);

        //Act
        var result = await _sessionService.GetSessionByIdAsync(testSessionId);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        var returnedSessionModel = Assert.IsType<ResponseResult<SessionModel>>(result);
        Assert.Equal(testSessionModel.Id, returnedSessionModel.Data?.Id);
        Assert.Equal(testSessionModel.Title, returnedSessionModel.Data?.Title);
        Assert.Equal(testSessionModel.Description, returnedSessionModel.Data?.Description);
        Assert.Equal(testSessionModel.MaxParticipants, returnedSessionModel.Data?.MaxParticipants);
        Assert.Equal(testSessionModel.CurrentParticipants, returnedSessionModel.Data?.CurrentParticipants);
        Assert.Equal(testSessionModel.Date, returnedSessionModel.Data?.Date);
        Assert.Equal(200, result.StatusCode); //200 OK
    }

    [Fact]
    public async Task GetSessionById_ShouldReturnNotFound_WhenSessionDoesNotExist()
    {
        //Arrange
        var testSessionId = "nonexistent-session-id";

        _repositoryMock
    .Setup(repo => repo.GetAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>()))
    .ReturnsAsync((SessionEntity?)null);

        //Act
        var result = await _sessionService.GetSessionByIdAsync(testSessionId);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode); //404 NotFound
    }

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
    public async Task GetAllSessionsAsync_ShouldReturnStatusCode200_WhenSessionsExists()
    {
        //Arrange
        var entities = new List<SessionEntity>
            {
                new SessionEntity
                {
                    Id = "1fe1b233 - c90e - 492e-aa77 - 3b3c3d93bf0f",
                    Title = "Title1",
                    Description = "Description1",
                    MaxParticipants = 1,
                    CurrentParticipants = 1,
                    Date = DateTime.Now,
                },
                new SessionEntity
                {
                    Id = "51b613fe-541e-44e7-8774-94c3bdb076d5",
                    Title = "Title2",
                    Description = "Description2",
                    MaxParticipants = 1,
                    CurrentParticipants = 1,
                    Date = DateTime.Now,
                }
            };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(entities);

        //Act
        var result = await _sessionService.GetAllSessionsAsync();


        //Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetAllSessionsAsync_ShouldReturnListWithSessions_WhenSessionsExists()
    {
        //Arrange
        var entities = new List<SessionEntity>
            {
                new SessionEntity
                {
                    Id = "1fe1b233 - c90e - 492e-aa77 - 3b3c3d93bf0f",
                    Title = "Title1",
                    Description = "Description1",
                    MaxParticipants = 1,
                    CurrentParticipants = 1,
                    Date = DateTime.Now,
                },
                new SessionEntity
                {
                    Id = "51b613fe-541e-44e7-8774-94c3bdb076d5",
                    Title = "Title2",
                    Description = "Description2",
                    MaxParticipants = 1,
                    CurrentParticipants = 1,
                    Date = DateTime.Now,
                }
            };

        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(entities);

        //Act
        var baseResult = await _sessionService.GetAllSessionsAsync();


        //Assert
        var result = Assert.IsType<ResponseResult<IEnumerable<SessionModel>>>(baseResult);
        var sessions = Assert.IsAssignableFrom<IEnumerable<SessionModel>>(result.Data);
        Assert.Equal(2, result.Data?.Count());
    }

    [Fact]
    public async Task GetAllSessionsAsync_ShouldReturnNotFound_WhenNoSessionsExists()
    {
        //Arrange
        _repositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(Array.Empty<SessionEntity>());

        //Act
        var result = await _sessionService.GetAllSessionsAsync();

        //Assert
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }

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

    [Fact]
    public async Task UpdateSessionAsync_ShouldReturnBadRequest_WhenSessionIsNull()
    {
        //Arrange


        //Act
        var result = await _sessionService.UpdateSessionAsync(null!);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode); 
    }

    [Fact]
    public async Task UpdateSessionAsync_ShouldReturnNotfound_WhenRepoMethodGetReturnsNull()
    {
        var testSessionModel = new SessionModel
        {
            Id = "testSessionId",
            Title = "Test Session",
            Description = "Test Description",
            MaxParticipants = 10,
            CurrentParticipants = 0,
            Date = DateTime.Now
        };
        //Arrange
        _repositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>()))
            .ReturnsAsync((SessionEntity?)null);

        //Act
        var result = await _sessionService.UpdateSessionAsync(testSessionModel);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);     
    }


    [Theory]
    [InlineData(5, 10, 10, 8)] // MaxParticipants < CurrentParticipants
    public async Task UpdateSessionAsync_ShouldReturnBadRequest_WhenMaxParticipantsIsUnderCurrentParticipants(int newMaxPart, int newCurrPart, int oldMaxPart, int oldCurrPart)
    {
        var newSessionModel_test = new SessionModel
        {
            Id = "testSessionId",
            Title = "Test Session",
            Description = "Test Description",
            MaxParticipants = newMaxPart,
            CurrentParticipants = newCurrPart,
            Date = DateTime.Now
        };
        var oldSessionEntity_test = new SessionEntity
        {
            Id = "testSessionId",
            Title = "Test Session",
            Description = "Test Description",
            MaxParticipants = oldMaxPart,
            CurrentParticipants = oldCurrPart,
            Date = DateTime.Now
        };
        //Arrange
        _repositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>()))
            .ReturnsAsync(oldSessionEntity_test);

        //Act
        var result = await _sessionService.UpdateSessionAsync(newSessionModel_test);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact] 
    public async Task UpdateSessionAsync_ShouldReturnOk_WhenUpdateModelIsValidAndRepoDoesNotThrow()
    {
        var newSessionModel_test = new SessionModel
        {
            Id = "testSessionId_new",
            Title = "Test Session_new",
            Description = "Test Description_new",
            MaxParticipants = 10,
            CurrentParticipants = 8,
            Date = DateTime.Now
        };
        var oldSessionEntity_test = new SessionEntity
        {
            Id = "testSessionId",
            Title = "Test Session",
            Description = "Test Description",
            MaxParticipants = 15,
            CurrentParticipants = 7,
            Date = DateTime.Now
        };
        //Arrange
        _repositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>()))
            .ReturnsAsync(oldSessionEntity_test);
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>(), It.IsAny<SessionEntity>()))
            .ReturnsAsync(true);

        //Act
        var result = await _sessionService.UpdateSessionAsync(newSessionModel_test);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
    }

    [Fact]
    public async Task UpdateSessionAsync_ShouldReturnError_WhenRepoReturnsFalse()
    {
        var newSessionModel_test = new SessionModel
        {
            Id = "testSessionId_new",
            Title = "Test Session_new",
            Description = "Test Description_new",
            MaxParticipants = 10,
            CurrentParticipants = 8,
            Date = DateTime.Now,
            Intensity = "Low"
        };
        var oldSessionEntity_test = new SessionEntity
        {
            Id = "testSessionId",
            Title = "Test Session",
            Description = "Test Description",
            MaxParticipants = 15,
            CurrentParticipants = 7,
            Date = DateTime.Now,
            Intensity = "High"
        };
        //Arrange
        _repositoryMock
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>()))
            .ReturnsAsync(oldSessionEntity_test);
        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<Expression<Func<SessionEntity, bool>>>(), It.IsAny<SessionEntity>()))
            .ReturnsAsync(false);

        //Act
        var result = await _sessionService.UpdateSessionAsync(newSessionModel_test);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
    }
}