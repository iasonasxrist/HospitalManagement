using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using HospitalApi.Controllers;
using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;
using HospitalApi.Models;

namespace HospitalApi.Tests
{
    public class NotificationsControllerTests
    {
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly NotificationsController _controller;

        public NotificationsControllerTests()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _controller = new NotificationsController(_mockNotificationService.Object);
        }

        [Fact]
        public async Task GetNotifications_ShouldReturnOkResult_WhenNotificationsExist()
        {
            // Arrange
            var expectedNotifications = new List<NotificationResponseDto>
            {
                new NotificationResponseDto { Id = 1, Message = "Test notification 1", IsRead = false },
                new NotificationResponseDto { Id = 2, Message = "Test notification 2", IsRead = true }
            };
            _mockNotificationService.Setup(x => x.GetNotificationsAsync(It.IsAny<int?>(), It.IsAny<bool?>()))
                .ReturnsAsync(expectedNotifications);

            // Act
            var result = await _controller.GetNotifications();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedNotifications = okResult.Value.Should().BeOfType<List<NotificationResponseDto>>().Subject;
            returnedNotifications.Should().HaveCount(2);
            returnedNotifications.Should().BeEquivalentTo(expectedNotifications);
        }

        [Fact]
        public async Task GetNotifications_ShouldReturnFilteredResults_WhenFiltersAreApplied()
        {
            // Arrange
            var userId = 1;
            var isRead = false;
            var expectedNotifications = new List<NotificationResponseDto>
            {
                new NotificationResponseDto { Id = 1, Message = "Unread notification", IsRead = false, UserId = userId }
            };
            _mockNotificationService.Setup(x => x.GetNotificationsAsync(userId, isRead))
                .ReturnsAsync(expectedNotifications);

            // Act
            var result = await _controller.GetNotifications(userId: userId, isRead: isRead);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedNotifications = okResult.Value.Should().BeOfType<List<NotificationResponseDto>>().Subject;
            returnedNotifications.Should().HaveCount(1);
            returnedNotifications.Should().BeEquivalentTo(expectedNotifications);
        }

        [Fact]
        public async Task GetNotification_ShouldReturnOkResult_WhenNotificationExists()
        {
            // Arrange
            var notificationId = 1;
            var allNotifications = new List<NotificationResponseDto>
            {
                new NotificationResponseDto { Id = 1, Message = "Test notification", IsRead = false },
                new NotificationResponseDto { Id = 2, Message = "Another notification", IsRead = true }
            };
            var expectedNotification = allNotifications.First(n => n.Id == notificationId);
            _mockNotificationService.Setup(x => x.GetNotificationsAsync())
                .ReturnsAsync(allNotifications);

            // Act
            var result = await _controller.GetNotification(notificationId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedNotification = okResult.Value.Should().BeOfType<NotificationResponseDto>().Subject;
            returnedNotification.Should().BeEquivalentTo(expectedNotification);
        }

        [Fact]
        public async Task GetNotification_ShouldReturnNotFound_WhenNotificationDoesNotExist()
        {
            // Arrange
            var notificationId = 999;
            var allNotifications = new List<NotificationResponseDto>
            {
                new NotificationResponseDto { Id = 1, Message = "Test notification", IsRead = false }
            };
            _mockNotificationService.Setup(x => x.GetNotificationsAsync())
                .ReturnsAsync(allNotifications);

            // Act
            var result = await _controller.GetNotification(notificationId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateNotification_ShouldReturnCreatedResult_WhenNotificationIsCreatedSuccessfully()
        {
            // Arrange
            var createNotificationDto = new CreateNotificationDto 
            { 
                Message = "New notification", 
                UserId = 1, 
                Priority = NotificationPriority.Normal 
            };
            var createdNotification = new NotificationResponseDto { Id = 1, Message = "New notification", UserId = 1 };
            _mockNotificationService.Setup(x => x.CreateNotificationAsync(createNotificationDto))
                .ReturnsAsync(createdNotification);

            // Act
            var result = await _controller.CreateNotification(createNotificationDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnedNotification = createdResult.Value.Should().BeOfType<NotificationResponseDto>().Subject;
            returnedNotification.Should().BeEquivalentTo(createdNotification);
            createdResult.ActionName.Should().Be(nameof(NotificationsController.GetNotification));
        }

        [Fact]
        public async Task MarkAsRead_ShouldReturnOkResult_WhenNotificationIsMarkedAsReadSuccessfully()
        {
            // Arrange
            var notificationId = 1;
            var markedNotification = new NotificationResponseDto { Id = notificationId, Message = "Test notification", IsRead = true };
            _mockNotificationService.Setup(x => x.MarkAsReadAsync(notificationId))
                .ReturnsAsync(markedNotification);

            // Act
            var result = await _controller.MarkAsRead(notificationId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedNotification = okResult.Value.Should().BeOfType<NotificationResponseDto>().Subject;
            returnedNotification.Should().BeEquivalentTo(markedNotification);
        }

        [Fact]
        public async Task MarkAsRead_ShouldReturnNotFound_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var notificationId = 999;
            _mockNotificationService.Setup(x => x.MarkAsReadAsync(notificationId))
                .ThrowsAsync(new ArgumentException("Notification not found"));

            // Act
            var result = await _controller.MarkAsRead(notificationId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetUnreadNotifications_ShouldReturnOkResult_WhenUnreadNotificationsExist()
        {
            // Arrange
            var userId = 1;
            var expectedUnreadNotifications = new List<NotificationResponseDto>
            {
                new NotificationResponseDto { Id = 1, Message = "Unread notification 1", IsRead = false, UserId = userId },
                new NotificationResponseDto { Id = 2, Message = "Unread notification 2", IsRead = false, UserId = userId }
            };
            _mockNotificationService.Setup(x => x.GetNotificationsAsync(userId, false))
                .ReturnsAsync(expectedUnreadNotifications);

            // Act
            var result = await _controller.GetUnreadNotifications(userId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedNotifications = okResult.Value.Should().BeOfType<List<NotificationResponseDto>>().Subject;
            returnedNotifications.Should().HaveCount(2);
            returnedNotifications.Should().BeEquivalentTo(expectedUnreadNotifications);
        }

        [Fact]
        public async Task GetCriticalNotifications_ShouldReturnOkResult_WhenCriticalNotificationsExist()
        {
            // Arrange
            var userId = 1;
            var allNotifications = new List<NotificationResponseDto>
            {
                new NotificationResponseDto { Id = 1, Message = "Critical notification", Priority = NotificationPriority.Critical, UserId = userId },
                new NotificationResponseDto { Id = 2, Message = "Normal notification", Priority = NotificationPriority.Normal, UserId = userId },
                new NotificationResponseDto { Id = 3, Message = "Another critical", Priority = NotificationPriority.Critical, UserId = userId }
            };
            var expectedCriticalNotifications = allNotifications.Where(n => n.Priority == NotificationPriority.Critical).ToList();
            _mockNotificationService.Setup(x => x.GetNotificationsAsync(userId))
                .ReturnsAsync(allNotifications);

            // Act
            var result = await _controller.GetCriticalNotifications(userId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedNotifications = okResult.Value.Should().BeOfType<List<NotificationResponseDto>>().Subject;
            returnedNotifications.Should().HaveCount(2);
            returnedNotifications.Should().BeEquivalentTo(expectedCriticalNotifications);
        }

        [Fact]
        public async Task MarkAllAsRead_ShouldReturnNoContent_WhenAllNotificationsAreMarkedAsRead()
        {
            // Arrange
            var userId = 1;
            var unreadNotifications = new List<NotificationResponseDto>
            {
                new NotificationResponseDto { Id = 1, Message = "Unread notification 1", IsRead = false, UserId = userId },
                new NotificationResponseDto { Id = 2, Message = "Unread notification 2", IsRead = false, UserId = userId }
            };
            _mockNotificationService.Setup(x => x.GetNotificationsAsync(userId, false))
                .ReturnsAsync(unreadNotifications);
            _mockNotificationService.Setup(x => x.MarkAsReadAsync(It.IsAny<int>()))
                .ReturnsAsync((NotificationResponseDto)null);

            // Act
            var result = await _controller.MarkAllAsRead(userId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockNotificationService.Verify(x => x.MarkAsReadAsync(1), Times.Once);
            _mockNotificationService.Verify(x => x.MarkAsReadAsync(2), Times.Once);
        }

        [Fact]
        public async Task DeleteNotification_ShouldReturnNoContent_WhenNotificationIsDeletedSuccessfully()
        {
            // Arrange
            var notificationId = 1;
            _mockNotificationService.Setup(x => x.MarkAsReadAsync(notificationId))
                .ReturnsAsync((NotificationResponseDto)null);

            // Act
            var result = await _controller.DeleteNotification(notificationId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeleteNotification_ShouldReturnNotFound_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var notificationId = 999;
            _mockNotificationService.Setup(x => x.MarkAsReadAsync(notificationId))
                .ThrowsAsync(new ArgumentException("Notification not found"));

            // Act
            var result = await _controller.DeleteNotification(notificationId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
} 