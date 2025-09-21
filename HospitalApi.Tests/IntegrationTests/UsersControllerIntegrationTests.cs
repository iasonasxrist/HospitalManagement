using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using HospitalApi.Application.DTOs;
using HospitalApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using HospitalApi.Data;
using HospitalApi.Infrastructure.Interfaces.Services;
using Moq;

namespace HospitalApi.Tests.IntegrationTests
{
    public class UsersControllerIntegrationTests : IClassFixture<TestFixture>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public UsersControllerIntegrationTests(TestFixture factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOkResult_WhenUsersExist()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HospitalContext>();
            
            // Add test users to the database
            var testUsers = new List<User>
            {
                new User { Username = "admin", PasswordHash = "hash", Role = UserRole.Admin },
                new User { Username = "doctor", PasswordHash = "hash", Role = UserRole.Doctor }
            };
            context.Users.AddRange(testUsers);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/users");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var users = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
            users.Should().NotBeNull();
            users.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetUser_ShouldReturnOkResult_WhenUserExists()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HospitalContext>();
            
            var testUser = new User { Username = "testuser", PasswordHash = "hash", Role = UserRole.Doctor };
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/users/{testUser.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            user.Should().NotBeNull();
            user.Username.Should().Be("testuser");
        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Act
            var response = await _client.GetAsync("/api/users/999");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedResult_WhenUserIsCreatedSuccessfully()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "newuser",
                Password = "password123",
                Role = UserRole.Nurse
            };
            var json = JsonSerializer.Serialize(createUserDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            user.Should().NotBeNull();
            user.Username.Should().Be("newuser");
            user.Role.Should().Be(UserRole.Nurse);
        }

        [Fact]
        public async Task Login_ShouldReturnOkResult_WhenLoginIsSuccessful()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HospitalContext>();
            
            var testUser = new User { Username = "loginuser", PasswordHash = "hash", Role = UserRole.Doctor };
            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Username = "loginuser",
                Password = "password"
            };
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var user = await response.Content.ReadFromJsonAsync<UserResponseDto>();
            user.Should().NotBeNull();
            user.Username.Should().Be("loginuser");
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenLoginFails()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "wronguser",
                Password = "wrongpassword"
            };
            var json = JsonSerializer.Serialize(loginDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/users/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task GetDoctors_ShouldReturnOkResult_WhenDoctorsExist()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HospitalContext>();
            
            var testUsers = new List<User>
            {
                new User { Username = "doctor1", PasswordHash = "hash", Role = UserRole.Doctor },
                new User { Username = "doctor2", PasswordHash = "hash", Role = UserRole.Doctor },
                new User { Username = "nurse1", PasswordHash = "hash", Role = UserRole.Nurse }
            };
            context.Users.AddRange(testUsers);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/users/doctors");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var doctors = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
            doctors.Should().NotBeNull();
            doctors.Should().HaveCount(2);
            doctors.Should().OnlyContain(d => d.Role == UserRole.Doctor);
        }

        [Fact]
        public async Task GetNurses_ShouldReturnOkResult_WhenNursesExist()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<HospitalContext>();
            
            var testUsers = new List<User>
            {
                new User { Username = "nurse1", PasswordHash = "hash", Role = UserRole.Nurse },
                new User { Username = "nurse2", PasswordHash = "hash", Role = UserRole.Nurse },
                new User { Username = "doctor1", PasswordHash = "hash", Role = UserRole.Doctor }
            };
            context.Users.AddRange(testUsers);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/users/nurses");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var nurses = await response.Content.ReadFromJsonAsync<List<UserResponseDto>>();
            nurses.Should().NotBeNull();
            nurses.Should().HaveCount(2);
            nurses.Should().OnlyContain(n => n.Role == UserRole.Nurse);
        }
    }
} 