using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using HospitalApi.Controllers;
using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;
using HospitalApi.Models;

namespace HospitalApi.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ILogger<UsersController>> _mockLogger;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<UsersController>>();
            _controller = new UsersController(_mockUserService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOkResult_WhenUsersExist()
        {
            // Arrange
            var expectedUsers = new List<UserResponseDto>
            {
                new UserResponseDto { Id = 1, Username = "admin", Role = UserRole.Admin },
                new UserResponseDto { Id = 2, Username = "doctor", Role = UserRole.Doctor }
            };
            _mockUserService.Setup(x => x.GetUsersAsync(It.IsAny<UserRole?>()))
                .ReturnsAsync(expectedUsers);

            // Act
            var result = await _controller.GetUsers();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUsers = okResult.Value.Should().BeOfType<List<UserResponseDto>>().Subject;
            returnedUsers.Should().HaveCount(2);
            returnedUsers.Should().BeEquivalentTo(expectedUsers);
        }

        [Fact]
        public async Task GetUser_ShouldReturnOkResult_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new UserResponseDto { Id = userId, Username = "testuser", Role = UserRole.Doctor };
            _mockUserService.Setup(x => x.GetUserAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUser = okResult.Value.Should().BeOfType<UserResponseDto>().Subject;
            returnedUser.Should().BeEquivalentTo(expectedUser);
        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            _mockUserService.Setup(x => x.GetUserAsync(userId))
                .ReturnsAsync((UserResponseDto?)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedResult_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Username = "newuser", Password = "password", Role = UserRole.Doctor };
            var createdUser = new UserResponseDto { Id = 1, Username = "newuser", Role = UserRole.Doctor };
            _mockUserService.Setup(x => x.CreateUserAsync(createUserDto))
                .ReturnsAsync(createdUser);

            // Act
            var result = await _controller.CreateUser(createUserDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnedUser = createdResult.Value.Should().BeOfType<UserResponseDto>().Subject;
            returnedUser.Should().BeEquivalentTo(createdUser);
            createdResult.ActionName.Should().Be(nameof(UsersController.GetUser));
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var createUserDto = new CreateUserDto { Username = "existinguser", Password = "password", Role = UserRole.Doctor };
            _mockUserService.Setup(x => x.CreateUserAsync(createUserDto))
                .ThrowsAsync(new ArgumentException("User already exists"));

            // Act
            var result = await _controller.CreateUser(createUserDto);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("User already exists");
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNoContent_WhenUserIsUpdatedSuccessfully()
        {
            // Arrange
            var userId = 1;
            var updateUserDto = new UpdateUserDto { Username = "updateduser", Role = UserRole.Nurse };
            _mockUserService.Setup(x => x.UpdateUserAsync(userId, updateUserDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateUser(userId, updateUserDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            var updateUserDto = new UpdateUserDto { Username = "updateduser", Role = UserRole.Nurse };
            _mockUserService.Setup(x => x.UpdateUserAsync(userId, updateUserDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateUser(userId, updateUserDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnNoContent_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999;
            _mockUserService.Setup(x => x.DeleteUserAsync(userId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Login_ShouldReturnOkResult_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "testuser", Password = "password" };
            var user = new UserResponseDto { Id = 1, Username = "testuser", Role = UserRole.Doctor };
            _mockUserService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedUser = okResult.Value.Should().BeOfType<UserResponseDto>().Subject;
            returnedUser.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenLoginFails()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "wronguser", Password = "wrongpassword" };
            _mockUserService.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync((UserResponseDto?)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = result.Result.Should().BeOfType<UnauthorizedObjectResult>().Subject;
            unauthorizedResult.Value.Should().Be("Invalid username or password");
        }

        [Fact]
        public async Task GetDoctors_ShouldReturnOkResult_WhenDoctorsExist()
        {
            // Arrange
            var expectedDoctors = new List<UserResponseDto>
            {
                new UserResponseDto { Id = 1, Username = "doctor1", Role = UserRole.Doctor },
                new UserResponseDto { Id = 2, Username = "doctor2", Role = UserRole.Doctor }
            };
            _mockUserService.Setup(x => x.GetDoctorsAsync())
                .ReturnsAsync(expectedDoctors);

            // Act
            var result = await _controller.GetDoctors();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedDoctors = okResult.Value.Should().BeOfType<List<UserResponseDto>>().Subject;
            returnedDoctors.Should().HaveCount(2);
            returnedDoctors.Should().BeEquivalentTo(expectedDoctors);
        }

        [Fact]
        public async Task GetNurses_ShouldReturnOkResult_WhenNursesExist()
        {
            // Arrange
            var expectedNurses = new List<UserResponseDto>
            {
                new UserResponseDto { Id = 1, Username = "nurse1", Role = UserRole.Nurse },
                new UserResponseDto { Id = 2, Username = "nurse2", Role = UserRole.Nurse }
            };
            _mockUserService.Setup(x => x.GetNursesAsync())
                .ReturnsAsync(expectedNurses);

            // Act
            var result = await _controller.GetNurses();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedNurses = okResult.Value.Should().BeOfType<List<UserResponseDto>>().Subject;
            returnedNurses.Should().HaveCount(2);
            returnedNurses.Should().BeEquivalentTo(expectedNurses);
        }
    }
}
