
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
using Data.Interfaces;
using Moq;

namespace Test.Business
{
    public class SessionServiceTest
    {
        private readonly Mock<ISessionRepository> _sessionsRepositoryMoq = new();
        private readonly ISessionService _sessionService;

        public SessionServiceTest()
        {
            _sessionService = new SessionService(_sessionsRepositoryMoq.Object);
        }

        [Fact]
        public async Task GetAllSessionsAsync_ShouldReturnNotFound_WhenNoSessionsExists()
        {
            //Arrange
            _sessionsRepositoryMoq
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(Array.Empty<SessionEntity>());

            //Act
            var result = await _sessionService.GetAllSessionsAsync();

            //Assert
            Assert.False(result.Success);
            Assert.Equal(404, result.StatusCode);
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

            _sessionsRepositoryMoq
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

            _sessionsRepositoryMoq
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(entities);

            //Act
            var result = await _sessionService.GetAllSessionsAsync();


            //Assert
            Assert.True(result.Success);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(result);
        }
    }
}
