
using System.Net;
using System.Net.Http.Json;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;


namespace LocalizationDemo.Api.Tests;

public class WeatherForecastTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string[] _englishSummaries =
        ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    public WeatherForecastTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetWeatherForecast_Returns200_RetrievesItems()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        Assert.NotNull(forecasts);
        Assert.NotEmpty(forecasts);
    }

    [Fact]
    public async Task GetWeatherForecast_Localization_Works()
    {

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Accept-Language", "hu-HU");
        var response = await client.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        Assert.NotNull(forecasts);
        Assert.All(forecasts, f => Assert.DoesNotContain(f.Summary, _englishSummaries));
    }

    [Fact]
    public async Task GetWeatherForecast_UnsupportedCulture_FallsBackToDefault()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("Accept-Language", "fr-FR");
        var response = await client.GetAsync("/weatherforecast");
        response.EnsureSuccessStatusCode();
        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        Assert.NotNull(forecasts);
        Assert.All(forecasts, f => Assert.Contains(f.Summary, _englishSummaries));
    }

    public class WeatherForecast
    {
        public DateOnly Date { get; set; }
        public int TemperatureC { get; set; }
        public int TemperatureF { get; set; }
        public string? Summary { get; set; }
    }
}


