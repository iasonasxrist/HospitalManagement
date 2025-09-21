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
    public class PatientsControllerTests
    {
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _mockPatientService = new Mock<IPatientService>();
            _controller = new PatientsController(_mockPatientService.Object);
        }

        [Fact]
        public async Task GetPatients_ShouldReturnOkResult_WhenPatientsExist()
        {
            // Arrange
            var expectedPatients = new List<PatientResponseDto>
            {
                new PatientResponseDto { Id = 1, FirstName = "John", LastName = "Doe", IsCritical = false },
                new PatientResponseDto { Id = 2, FirstName = "Jane", LastName = "Smith", IsCritical = true }
            };
            _mockPatientService.Setup(x => x.GetPatientsAsync(It.IsAny<bool?>(), It.IsAny<PatientStatus?>(), It.IsAny<string>()))
                .ReturnsAsync(expectedPatients);

            // Act
            var result = await _controller.GetPatients();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPatients = okResult.Value.Should().BeOfType<List<PatientResponseDto>>().Subject;
            returnedPatients.Should().HaveCount(2);
            returnedPatients.Should().BeEquivalentTo(expectedPatients);
        }

        [Fact]
        public async Task GetPatients_ShouldReturnFilteredResults_WhenFiltersAreApplied()
        {
            // Arrange
            var expectedPatients = new List<PatientResponseDto>
            {
                new PatientResponseDto { Id = 1, FirstName = "John", LastName = "Doe", IsCritical = true }
            };
            _mockPatientService.Setup(x => x.GetPatientsAsync(true, PatientStatus.Admitted, "John"))
                .ReturnsAsync(expectedPatients);

            // Act
            var result = await _controller.GetPatients(isCritical: true, status: PatientStatus.Admitted, search: "John");

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPatients = okResult.Value.Should().BeOfType<List<PatientResponseDto>>().Subject;
            returnedPatients.Should().HaveCount(1);
            returnedPatients.Should().BeEquivalentTo(expectedPatients);
        }

        [Fact]
        public async Task GetPatient_ShouldReturnOkResult_WhenPatientExists()
        {
            // Arrange
            var patientId = 1;
            var expectedPatient = new PatientResponseDto { Id = patientId, FirstName = "John", LastName = "Doe" };
            _mockPatientService.Setup(x => x.GetPatientAsync(patientId))
                .ReturnsAsync(expectedPatient);

            // Act
            var result = await _controller.GetPatient(patientId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPatient = okResult.Value.Should().BeOfType<PatientResponseDto>().Subject;
            returnedPatient.Should().BeEquivalentTo(expectedPatient);
        }

        [Fact]
        public async Task GetPatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = 999;
            _mockPatientService.Setup(x => x.GetPatientAsync(patientId))
                .ReturnsAsync((PatientResponseDto?)null);

            // Act
            var result = await _controller.GetPatient(patientId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreatePatient_ShouldReturnCreatedResult_WhenPatientIsCreatedSuccessfully()
        {
            // Arrange
            var createPatientDto = new CreatePatientDto 
            { 
                FirstName = "John", 
                LastName = "Doe", 
                DateOfBirth = DateTime.Now.AddYears(-30),
                Gender = Gender.Male,
                ContactNumber = "1234567890"
            };
            var createdPatient = new PatientResponseDto { Id = 1, FirstName = "John", LastName = "Doe" };
            _mockPatientService.Setup(x => x.CreatePatientAsync(createPatientDto))
                .ReturnsAsync(createdPatient);

            // Act
            var result = await _controller.CreatePatient(createPatientDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnedPatient = createdResult.Value.Should().BeOfType<PatientResponseDto>().Subject;
            returnedPatient.Should().BeEquivalentTo(createdPatient);
            createdResult.ActionName.Should().Be(nameof(PatientsController.GetPatient));
        }

        [Fact]
        public async Task UpdatePatient_ShouldReturnNoContent_WhenPatientIsUpdatedSuccessfully()
        {
            // Arrange
            var patientId = 1;
            var updatePatientDto = new UpdatePatientDto { FirstName = "Updated", LastName = "Name" };
            _mockPatientService.Setup(x => x.UpdatePatientAsync(patientId, updatePatientDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdatePatient(patientId, updatePatientDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task UpdatePatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = 999;
            var updatePatientDto = new UpdatePatientDto { FirstName = "Updated", LastName = "Name" };
            _mockPatientService.Setup(x => x.UpdatePatientAsync(patientId, updatePatientDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdatePatient(patientId, updatePatientDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeletePatient_ShouldReturnNoContent_WhenPatientIsDeletedSuccessfully()
        {
            // Arrange
            var patientId = 1;
            _mockPatientService.Setup(x => x.DeletePatientAsync(patientId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePatient(patientId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task DeletePatient_ShouldReturnNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = 999;
            _mockPatientService.Setup(x => x.DeletePatientAsync(patientId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeletePatient(patientId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetCriticalPatients_ShouldReturnOkResult_WhenCriticalPatientsExist()
        {
            // Arrange
            var expectedCriticalPatients = new List<PatientResponseDto>
            {
                new PatientResponseDto { Id = 1, FirstName = "John", LastName = "Doe", IsCritical = true },
                new PatientResponseDto { Id = 2, FirstName = "Jane", LastName = "Smith", IsCritical = true }
            };
            _mockPatientService.Setup(x => x.GetCriticalPatientsAsync())
                .ReturnsAsync(expectedCriticalPatients);

            // Act
            var result = await _controller.GetCriticalPatients();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedPatients = okResult.Value.Should().BeOfType<List<PatientResponseDto>>().Subject;
            returnedPatients.Should().HaveCount(2);
            returnedPatients.Should().BeEquivalentTo(expectedCriticalPatients);
        }

        [Fact]
        public async Task MarkPatientCritical_ShouldReturnNoContent_WhenPatientIsMarkedCriticalSuccessfully()
        {
            // Arrange
            var patientId = 1;
            var reason = "Severe condition";
            _mockPatientService.Setup(x => x.MarkPatientCriticalAsync(patientId, reason))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.MarkPatientCritical(patientId, reason);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task MarkPatientCritical_ShouldReturnNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = 999;
            var reason = "Severe condition";
            _mockPatientService.Setup(x => x.MarkPatientCriticalAsync(patientId, reason))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.MarkPatientCritical(patientId, reason);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task MarkPatientStable_ShouldReturnNoContent_WhenPatientIsMarkedStableSuccessfully()
        {
            // Arrange
            var patientId = 1;
            _mockPatientService.Setup(x => x.MarkPatientStableAsync(patientId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.MarkPatientStable(patientId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task MarkPatientStable_ShouldReturnNotFound_WhenPatientDoesNotExist()
        {
            // Arrange
            var patientId = 999;
            _mockPatientService.Setup(x => x.MarkPatientStableAsync(patientId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.MarkPatientStable(patientId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
} 