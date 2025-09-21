using Microsoft.EntityFrameworkCore;
using HospitalApi.Data;
using HospitalApi.Infrastructure.Repositories;
using HospitalApi.Infrastructure.Interfaces.Services;
using HospitalApi.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext for PostgreSQL
// builder.Services.AddDbContext<HospitalContext>(options =>
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add DbContext for InMemory
builder.Services.AddDbContext<HospitalContext>(options =>
    options.UseInMemoryDatabase("HospitalDb"));

// Add services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IVitalSignService, VitalSignService>();
builder.Services.AddRepositories();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Map controllers
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HospitalContext>();
    context.Database.EnsureCreated();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Make Program class public for testing
public partial class Program { }
