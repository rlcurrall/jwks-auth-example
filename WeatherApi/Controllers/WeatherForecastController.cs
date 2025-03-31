using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WeatherApi.Models;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(ILogger<WeatherForecastController> logger) : ControllerBase
{
    private static readonly string[] _summaries =
    [
        "Freezing",
        "Bracing",
        "Chilly",
        "Cool",
        "Mild",
        "Warm",
        "Balmy",
        "Hot",
        "Sweltering",
        "Scorching",
    ];

    /// <summary>
    /// Gets weather forecast data for the next 5 days.
    /// This endpoint requires authentication.
    /// </summary>
    /// <returns>Array of weather forecasts</returns>
    [HttpGet]
    [Authorize] // Requires authentication
    public IEnumerable<WeatherForecast> Get()
    {
        logger.LogInformation(
            "Authenticated user {UserId} requested weather forecast",
            User.FindFirst("sub")?.Value ?? "unknown"
        );

        return
        [
            .. Enumerable
                .Range(1, 5)
                .Select(index => new WeatherForecast(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    _summaries[Random.Shared.Next(_summaries.Length)]
                )),
        ];
    }

    /// <summary>
    /// Gets a detailed weather forecast for a specific day.
    /// Requires the "read" scope.
    /// </summary>
    /// <param name="id">Day offset from today</param>
    /// <returns>A detailed weather forecast</returns>
    [HttpGet("{id}")]
    [Authorize(Policy = "ReadAccess")] // Requires the "read" scope
    public ActionResult<WeatherForecast> GetById(int id)
    {
        if (id is < 1 or > 5)
        {
            return BadRequest("Forecast ID must be between 1 and 5");
        }

        var forecast = new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(id)),
            Random.Shared.Next(-20, 55),
            _summaries[Random.Shared.Next(_summaries.Length)]
        );

        return forecast;
    }

    /// <summary>
    /// Updates a weather forecast.
    /// Requires the "write" scope.
    /// </summary>
    /// <param name="id">Day offset from today</param>
    /// <param name="forecast">Updated forecast data</param>
    /// <returns>The updated forecast</returns>
    [HttpPut("{id}")]
    [Authorize(Policy = "WriteAccess")] // Requires the "write" scope
    public ActionResult<WeatherForecast> Update(int id, WeatherForecast forecast)
    {
        if (id is < 1 or > 5)
        {
            return BadRequest("Forecast ID must be between 1 and 5");
        }

        // In a real implementation, this would update data in a database
        // Here we just return the provided forecast with the actual date for the ID

        var updatedForecast = forecast with
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(id)),
        };

        return updatedForecast;
    }
}
