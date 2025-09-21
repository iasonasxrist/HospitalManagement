using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using HospitalApi.Data;
using HospitalApi.Infrastructure.Interfaces.Services;
using Moq;

namespace HospitalApi.Tests.IntegrationTests
{
    public class TestFixture : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<HospitalContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<HospitalContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Mock external services if needed
                var mockNotificationService = new Mock<INotificationService>();
                services.Replace(ServiceDescriptor.Singleton<INotificationService>(mockNotificationService.Object));
            });
        }
    }
} 