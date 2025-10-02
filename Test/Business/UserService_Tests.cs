using Business.Dtos;
using Business.Interfaces;
using Business.Models;
using Business.Services;
using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MockQueryable;
using MockQueryable.Moq;
using Moq;

namespace Test.Business;

public class UserService_Tests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly Mock<RoleManager<IdentityRole<Guid>>> _roleManagerMock;
    private readonly IUserService _userService;

    public UserService_Tests()
    {
        // Setup UserManager mock with all required dependencies
        var userStore = Mock.Of<IUserStore<UserEntity>>();
        var passwordHasher = Mock.Of<IPasswordHasher<UserEntity>>();
        var userValidators = new List<IUserValidator<UserEntity>>();
        var passwordValidators = new List<IPasswordValidator<UserEntity>>();
        var keyNormalizer = Mock.Of<ILookupNormalizer>();
        var errors = Mock.Of<IdentityErrorDescriber>();
        var services = Mock.Of<IServiceProvider>();
        var logger = Mock.Of<Microsoft.Extensions.Logging.ILogger<UserManager<UserEntity>>>();

        _userManagerMock = new Mock<UserManager<UserEntity>>(
            userStore, Mock.Of<IOptions<IdentityOptions>>(), passwordHasher, userValidators,
            passwordValidators, keyNormalizer, errors, services, logger);

        // Setup RoleManager mock
        var roleStore = Mock.Of<IRoleStore<IdentityRole<Guid>>>();
        var roleValidators = new List<IRoleValidator<IdentityRole<Guid>>>();
        var keyNormalizer2 = Mock.Of<ILookupNormalizer>();
        var errors2 = Mock.Of<IdentityErrorDescriber>();
        var loggerRole = Mock.Of<Microsoft.Extensions.Logging.ILogger<RoleManager<IdentityRole<Guid>>>>();

        _roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
            roleStore, roleValidators, keyNormalizer2, errors2, loggerRole);

        _userService = new UserService(_userManagerMock.Object, _roleManagerMock.Object);
    }

    #region CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_ShouldReturnBadRequest_WhenFormIsNull()
    {
        // Act
        var result = await _userService.CreateUserAsync(null!);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Invalid user data.", result.ResultMessage);
    }

    [Theory]
    [InlineData(null, "password123", "Test", "User")]
    [InlineData("", "password123", "Test", "User")]
    [InlineData("   ", "password123", "Test", "User")]
    [InlineData("test@test.com", null, "Test", "User")]
    [InlineData("test@test.com", "", "Test", "User")]
    [InlineData("test@test.com", "   ", "Test", "User")]
    public async Task CreateUserAsync_ShouldReturnBadRequest_WhenEmailOrPasswordIsInvalid(
        string? email, string? password, string firstName, string lastName)
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = email!,
            Password = password!,
            FirstName = firstName,
            LastName = lastName,
            ConfirmPassword = password!
        };

        // Act
        var result = await _userService.CreateUserAsync(form);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
        Assert.Equal("Email and password are required.", result.ResultMessage);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnAlreadyExists_WhenUserWithEmailExists()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "existing@test.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User",
            ConfirmPassword = "password123"
        };

        var existingUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            Email = "existing@test.com",
            FirstName = "Existing",
            LastName = "User"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync(existingUser);

        // Act
        var result = await _userService.CreateUserAsync(form);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
        Assert.Equal("A user with this email already exists.", result.ResultMessage);

        // Verify that no user creation was attempted
        _userManagerMock.Verify(x => x.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateFirstUserAsAdmin_WhenNoUsersExist()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "admin@test.com",
            Password = "password123",
            FirstName = "Admin",
            LastName = "User",
            ConfirmPassword = "password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync((UserEntity?)null);

        // ✅ Correct syntax for MockQueryable.Moq 8.0.0
        var emptyUsers = new List<UserEntity>().BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(emptyUsers);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), form.Password))
                       .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "Admin"))
                       .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.CreateUserAsync(form);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);

        // Verify user was created with correct data
        _userManagerMock.Verify(x => x.CreateAsync(It.Is<UserEntity>(u =>
            u.Email == form.Email &&
            u.UserName == form.Email &&
            u.FirstName == form.FirstName &&
            u.LastName == form.LastName), form.Password), Times.Once);

        // Verify admin role assignment
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "Admin"), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "User"), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateRegularUser_WhenUsersAlreadyExist()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "user@test.com",
            Password = "password123",
            FirstName = "Regular",
            LastName = "User",
            ConfirmPassword = "password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync((UserEntity?)null);

        // ✅ Correct syntax for MockQueryable.Moq 8.0.0
        var existingUsers = new List<UserEntity>
        {
            new UserEntity { Id = Guid.NewGuid(), Email = "existing@test.com", FirstName = "Existing", LastName = "User" }
        }.BuildMock();

        _userManagerMock.Setup(x => x.Users)
                       .Returns(existingUsers);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), form.Password))
                       .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "User"))
                       .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.CreateUserAsync(form);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);

        // Verify user role assignment (not admin)
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "User"), Times.Once);
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "Admin"), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnError_WhenUserCreationFails()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "test@test.com",
            Password = "weak",
            FirstName = "Test",
            LastName = "User",
            ConfirmPassword = "weak"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync((UserEntity?)null);

        var emptyUsers = new List<UserEntity>().BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(emptyUsers);

        var identityErrors = new[]
        {
            new IdentityError { Code = "PasswordTooShort", Description = "Password is too short" },
            new IdentityError { Code = "PasswordRequiresDigit", Description = "Password must contain a digit" }
        };

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), form.Password))
                       .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await _userService.CreateUserAsync(form);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("Password is too short", result.ResultMessage!);
        Assert.Contains("Password must contain a digit", result.ResultMessage!);

        // Verify no role assignment was attempted
        _userManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldDeleteUserAndReturnError_WhenAdminRoleAssignmentFails()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "admin@test.com",
            Password = "password123",
            FirstName = "Admin",
            LastName = "User",
            ConfirmPassword = "password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync((UserEntity?)null);

        var emptyUsers = new List<UserEntity>().BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(emptyUsers);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), form.Password))
                       .ReturnsAsync(IdentityResult.Success);

        var roleError = new IdentityError { Code = "RoleNotFound", Description = "Admin role does not exist" };
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "Admin"))
                       .ReturnsAsync(IdentityResult.Failed(roleError));

        _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<UserEntity>()))
                       .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.CreateUserAsync(form);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("User was not created - failed to assign roles", result.ResultMessage!);
        Assert.Contains("Admin role does not exist", result.ResultMessage!);

        // Verify user was deleted after role assignment failure
        _userManagerMock.Verify(x => x.DeleteAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldDeleteUserAndReturnError_WhenUserRoleAssignmentFails()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "user@test.com",
            Password = "password123",
            FirstName = "Regular",
            LastName = "User",
            ConfirmPassword = "password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync((UserEntity?)null);

        var existingUsers = new List<UserEntity>
        {
            new UserEntity { Id = Guid.NewGuid(), Email = "existing@test.com" }
        }.BuildMock();

        _userManagerMock.Setup(x => x.Users)
                       .Returns(existingUsers);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), form.Password))
                       .ReturnsAsync(IdentityResult.Success);

        var roleError = new IdentityError { Code = "RoleNotFound", Description = "User role does not exist" };
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "User"))
                       .ReturnsAsync(IdentityResult.Failed(roleError));

        _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<UserEntity>()))
                       .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.CreateUserAsync(form);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("User was not created - failed to assign roles", result.ResultMessage!);
        Assert.Contains("User role does not exist", result.ResultMessage!);

        // Verify user was deleted after role assignment failure
        _userManagerMock.Verify(x => x.DeleteAsync(It.IsAny<UserEntity>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnError_WhenMultipleRoleErrorsOccur()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "admin@test.com",
            Password = "password123",
            FirstName = "Admin",
            LastName = "User",
            ConfirmPassword = "password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync((UserEntity?)null);

        var emptyUsers = new List<UserEntity>().BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(emptyUsers);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), form.Password))
                       .ReturnsAsync(IdentityResult.Success);

        var roleErrors = new[]
        {
            new IdentityError { Code = "RoleNotFound", Description = "Role does not exist" },
            new IdentityError { Code = "RoleAssignmentFailed", Description = "Failed to assign role" }
        };
        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "Admin"))
                       .ReturnsAsync(IdentityResult.Failed(roleErrors));

        _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<UserEntity>()))
                       .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.CreateUserAsync(form);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(500, result.StatusCode);
        Assert.Contains("Role does not exist", result.ResultMessage!);
        Assert.Contains("Failed to assign role", result.ResultMessage!);
    }

    #endregion

    #region GetUsersAsync Tests

    [Fact]
    public async Task GetUsersAsync_ShouldReturnUsers_WhenUsersExist()
    {
        // Arrange
        var userEntities = new List<UserEntity>
        {
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = "user1@test.com",
                FirstName = "John",
                LastName = "Doe"
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = "user2@test.com",
                FirstName = "Jane",
                LastName = "Smith"
            }
        };

        // ✅ Correct syntax for MockQueryable.Moq 8.0.0
        var mockQueryable = userEntities.BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(mockQueryable);

        // Act
        var result = await _userService.GetUsersAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());

        var userList = result.Data.ToList();

        // Verify first user mapping
        Assert.Equal(userEntities[0].Id, userList[0].Id);
        Assert.Equal("user1@test.com", userList[0].Email);
        Assert.Equal("John", userList[0].FirstName);
        Assert.Equal("Doe", userList[0].LastName);

        // Verify second user mapping
        Assert.Equal(userEntities[1].Id, userList[1].Id);
        Assert.Equal("user2@test.com", userList[1].Email);
        Assert.Equal("Jane", userList[1].FirstName);
        Assert.Equal("Smith", userList[1].LastName);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange
        var emptyUsers = new List<UserEntity>().BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(emptyUsers);

        // Act
        var result = await _userService.GetUsersAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetUsersAsync_ShouldReturnCorrectCount_WhenMultipleUsersExist()
    {
        // Arrange
        var userEntities = new List<UserEntity>();
        for (int i = 1; i <= 10; i++)
        {
            userEntities.Add(new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = $"user{i}@test.com",
                FirstName = $"User{i}",
                LastName = "Test"
            });
        }

        var mockQueryable = userEntities.BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(mockQueryable);

        // Act
        var baseResult = await _userService.GetUsersAsync();

        // Assert
        var result = Assert.IsType<ResponseResult<IEnumerable<UserModel>>>(baseResult);
        var userModels = Assert.IsAssignableFrom<IEnumerable<UserModel>>(result.Data);
        Assert.Equal(10, result.Data?.Count());

        // Verify all users are properly mapped
        var userList = result.Data!.ToList();
        for (int i = 0; i < 10; i++)
        {
            Assert.Equal(userEntities[i].Id, userList[i].Id);
            Assert.Equal($"user{i + 1}@test.com", userList[i].Email);
            Assert.Equal($"User{i + 1}", userList[i].FirstName);
            Assert.Equal("Test", userList[i].LastName);
        }
    }

    [Fact]
    public async Task GetUsersAsync_ShouldHandleUsersWithSpecialCharacters()
    {
        // Arrange
        var userEntities = new List<UserEntity>
        {
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = "test.user+tag@example.com",
                FirstName = "José",
                LastName = "García-López"
            },
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = "user_with_underscore@test-domain.co.uk",
                FirstName = "François",
                LastName = "O'Connor"
            }
        };

        var mockQueryable = userEntities.BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(mockQueryable);

        // Act
        var result = await _userService.GetUsersAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(200, result.StatusCode);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data.Count());

        var userList = result.Data.ToList();

        // Verify special characters are preserved
        Assert.Equal("test.user+tag@example.com", userList[0].Email);
        Assert.Equal("José", userList[0].FirstName);
        Assert.Equal("García-López", userList[0].LastName);

        Assert.Equal("user_with_underscore@test-domain.co.uk", userList[1].Email);
        Assert.Equal("François", userList[1].FirstName);
        Assert.Equal("O'Connor", userList[1].LastName);
    }

    #endregion

    #region Integration-style Tests

    [Fact]
    public async Task CreateUserAsync_ShouldSetUserNameToEmail_WhenCreatingUser()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "test@example.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User",
            ConfirmPassword = "password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync((UserEntity?)null);

        var emptyUsers = new List<UserEntity>().BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(emptyUsers);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), form.Password))
                       .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "Admin"))
                       .ReturnsAsync(IdentityResult.Success);

        // Act
        await _userService.CreateUserAsync(form);

        // Assert
        _userManagerMock.Verify(x => x.CreateAsync(It.Is<UserEntity>(u =>
            u.UserName == form.Email), form.Password), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldUseUserFactoryForMapping()
    {
        // Arrange
        var form = new UserSignUpDto
        {
            Email = "factory@test.com",
            Password = "password123",
            FirstName = "Factory",
            LastName = "Test",
            ConfirmPassword = "password123"
        };

        _userManagerMock.Setup(x => x.FindByEmailAsync(form.Email))
                       .ReturnsAsync((UserEntity?)null);

        var emptyUsers = new List<UserEntity>().BuildMock();
        _userManagerMock.Setup(x => x.Users)
                       .Returns(emptyUsers);

        _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<UserEntity>(), form.Password))
                       .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<UserEntity>(), "Admin"))
                       .ReturnsAsync(IdentityResult.Success);

        // Act
        await _userService.CreateUserAsync(form);

        // Assert - Verify UserFactory.Create was used correctly
        _userManagerMock.Verify(x => x.CreateAsync(It.Is<UserEntity>(u =>
            u.FirstName == form.FirstName &&
            u.LastName == form.LastName &&
            u.Email == form.Email), form.Password), Times.Once);
    }

    #endregion
}