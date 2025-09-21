using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Mvc;
using HospitalApi.Controllers;
using HospitalApi.Application.DTOs;
using HospitalApi.Infrastructure.Interfaces.Services;

namespace HospitalApi.Tests
{
    public class VitalSignsControllerTests
    {
        private readonly Mock<IVitalSignService> _mockVitalSignService;
        private readonly VitalSignsController _controller;

        public VitalSignsControllerTests()
        {
            _mockVitalSignService = new Mock<IVitalSignService>();
            _controller = new VitalSignsController(_mockVitalSignService.Object);
        }

        [Fact]
        public async Task GetVitalSigns_ShouldReturnOkResult_WhenVitalSignsExist()
        {
            // Arrange
            var expectedVitalSigns = new List<VitalSignResponseDto>
            {
                new VitalSignResponseDto { Id = 1, PatientId = 1, BloodPressure = "120/80", HeartRate = 72 },
                new VitalSignResponseDto { Id = 2, PatientId = 2, BloodPressure = "140/90", HeartRate = 85 }
            };
            _mockVitalSignService.Setup(x => x.GetCriticalVitalSignsAsync())
                .ReturnsAsync(expectedVitalSigns);

            // Act
            var result = await _controller.GetVitalSigns();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedVitalSigns = okResult.Value.Should().BeOfType<List<VitalSignResponseDto>>().Subject;
            returnedVitalSigns.Should().HaveCount(2);
            returnedVitalSigns.Should().BeEquivalentTo(expectedVitalSigns);
        }

        [Fact]
        public async Task GetVitalSign_ShouldReturnOkResult_WhenVitalSignExists()
        {
            // Arrange
            var vitalSignId = 1;
            var expectedVitalSign = new VitalSignResponseDto { Id = vitalSignId, PatientId = 1, BloodPressure = "120/80", HeartRate = 72 };
            _mockVitalSignService.Setup(x => x.GetVitalSignAsync(vitalSignId))
                .ReturnsAsync(expectedVitalSign);

            // Act
            var result = await _controller.GetVitalSign(vitalSignId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedVitalSign = okResult.Value.Should().BeOfType<VitalSignResponseDto>().Subject;
            returnedVitalSign.Should().BeEquivalentTo(expectedVitalSign);
        }

        [Fact]
        public async Task GetVitalSign_ShouldReturnNotFound_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var vitalSignId = 999;
            _mockVitalSignService.Setup(x => x.GetVitalSignAsync(vitalSignId))
                .ThrowsAsync(new ArgumentException("Vital sign not found"));

            // Act
            var result = await _controller.GetVitalSign(vitalSignId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateVitalSign_ShouldReturnCreatedResult_WhenVitalSignIsCreatedSuccessfully()
        {
            // Arrange
            var createVitalSignDto = new CreateVitalSignDto 
            { 
                PatientId = 1, 
                BloodPressure = "120/80", 
                HeartRate = 72, 
                Temperature = 98.6m,
                RespiratoryRate = 16
            };
            var createdVitalSign = new VitalSignResponseDto { Id = 1, PatientId = 1, BloodPressure = "120/80", HeartRate = 72 };
            _mockVitalSignService.Setup(x => x.CreateVitalSignAsync(createVitalSignDto))
                .ReturnsAsync(createdVitalSign);

            // Act
            var result = await _controller.CreateVitalSign(createVitalSignDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnedVitalSign = createdResult.Value.Should().BeOfType<VitalSignResponseDto>().Subject;
            returnedVitalSign.Should().BeEquivalentTo(createdVitalSign);
            createdResult.ActionName.Should().Be(nameof(VitalSignsController.GetVitalSign));
        }

        [Fact]
        public async Task CreateVitalSign_ShouldReturnBadRequest_WhenArgumentExceptionIsThrown()
        {
            // Arrange
            var createVitalSignDto = new CreateVitalSignDto { PatientId = 999, BloodPressure = "120/80", HeartRate = 72 };
            _mockVitalSignService.Setup(x => x.CreateVitalSignAsync(createVitalSignDto))
                .ThrowsAsync(new ArgumentException("Patient not found"));

            // Act
            var result = await _controller.CreateVitalSign(createVitalSignDto);

            // Assert
            var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().Be("Patient not found");
        }

        [Fact]
        public async Task GetPatientVitalSigns_ShouldReturnOkResult_WhenPatientVitalSignsExist()
        {
            // Arrange
            var patientId = 1;
            var expectedVitalSigns = new List<VitalSignResponseDto>
            {
                new VitalSignResponseDto { Id = 1, PatientId = patientId, BloodPressure = "120/80", HeartRate = 72 },
                new VitalSignResponseDto { Id = 2, PatientId = patientId, BloodPressure = "118/78", HeartRate = 70 }
            };
            _mockVitalSignService.Setup(x => x.GetPatientVitalSignsAsync(patientId))
                .ReturnsAsync(expectedVitalSigns);

            // Act
            var result = await _controller.GetPatientVitalSigns(patientId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedVitalSigns = okResult.Value.Should().BeOfType<List<VitalSignResponseDto>>().Subject;
            returnedVitalSigns.Should().HaveCount(2);
            returnedVitalSigns.Should().BeEquivalentTo(expectedVitalSigns);
        }

        [Fact]
        public async Task GetCriticalVitalSigns_ShouldReturnOkResult_WhenCriticalVitalSignsExist()
        {
            // Arrange
            var expectedCriticalVitalSigns = new List<VitalSignResponseDto>
            {
                new VitalSignResponseDto { Id = 1, PatientId = 1, BloodPressure = "160/100", HeartRate = 110 },
                new VitalSignResponseDto { Id = 2, PatientId = 2, BloodPressure = "90/60", HeartRate = 45 }
            };
            _mockVitalSignService.Setup(x => x.GetCriticalVitalSignsAsync())
                .ReturnsAsync(expectedCriticalVitalSigns);

            // Act
            var result = await _controller.GetCriticalVitalSigns();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedVitalSigns = okResult.Value.Should().BeOfType<List<VitalSignResponseDto>>().Subject;
            returnedVitalSigns.Should().HaveCount(2);
            returnedVitalSigns.Should().BeEquivalentTo(expectedCriticalVitalSigns);
        }

        [Fact]
        public async Task GetLatestVitalSigns_ShouldReturnOkResult_WhenLatestVitalSignExists()
        {
            // Arrange
            var patientId = 1;
            var expectedVitalSigns = new List<VitalSignResponseDto>
            {
                new VitalSignResponseDto { Id = 2, PatientId = patientId, BloodPressure = "118/78", HeartRate = 70 },
                new VitalSignResponseDto { Id = 1, PatientId = patientId, BloodPressure = "120/80", HeartRate = 72 }
            };
            var expectedLatest = expectedVitalSigns.First();
            _mockVitalSignService.Setup(x => x.GetPatientVitalSignsAsync(patientId))
                .ReturnsAsync(expectedVitalSigns);

            // Act
            var result = await _controller.GetLatestVitalSigns(patientId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedVitalSign = okResult.Value.Should().BeOfType<VitalSignResponseDto>().Subject;
            returnedVitalSign.Should().BeEquivalentTo(expectedLatest);
        }

        [Fact]
        public async Task GetLatestVitalSigns_ShouldReturnNotFound_WhenNoVitalSignsExist()
        {
            // Arrange
            var patientId = 999;
            var emptyVitalSigns = new List<VitalSignResponseDto>();
            _mockVitalSignService.Setup(x => x.GetPatientVitalSignsAsync(patientId))
                .ReturnsAsync(emptyVitalSigns);

            // Act
            var result = await _controller.GetLatestVitalSigns(patientId);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }
    }
} 