using LocalizationDemo.Api.Localization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
services.AddOpenApi();

// Configure localization here, before the build section
services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add the Weather ResourceLocalizer service
services.AddScoped<IResourceLocalizer<WeatherResource>, ResourceLocalizer<WeatherResource>>();
// You can also add different resource localizers for different resources here, 
// but you must also register them before in the Markers.cs file,
// and create the corresponding resource files in the Resources folder
// e.g.:
// services.AddScoped<IResourceLocalizer<AnotherResource>, ResourceLocalizer<AnotherResource>>();

// Add the IStringLocalizer service
services.Configure<RequestLocalizationOptions>(options =>
{
    // Define the supported cultures
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("hu")
    };

    // Set the default culture as a fallback in case the requested culture is not supported
    options.DefaultRequestCulture = new RequestCulture("en-US");
    // Set the supported cultures and UI cultures
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders =
    [
        // Use the AcceptLanguageHeaderRequestCultureProvider to determine the request culture
        new AcceptLanguageHeaderRequestCultureProvider()
    ];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseRequestLocalization();

// This endpoint returns a list of weather forecasts in the current culture.
// The weather summaries are localized using the WeatherResource resource file.
app.MapGet("/weatherforecast", ([FromServices] IResourceLocalizer<WeatherResource> resourceLocalizer) =>
{
    // Generate a list of weather forecasts summarizations in the current culture
    var summaries = new[]
    {
        resourceLocalizer.Localize("Freezing"),
        resourceLocalizer.Localize("Bracing"),
        resourceLocalizer.Localize("Chilly"),
        resourceLocalizer.Localize("Cool"),
        resourceLocalizer.Localize("Mild"),
        resourceLocalizer.Localize("Warm"),
        resourceLocalizer.Localize("Balmy"),
        resourceLocalizer.Localize("Hot"),
        resourceLocalizer.Localize("Sweltering"),
        resourceLocalizer.Localize("Scorching")
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return Results.Ok(forecast);
})
// Add metadata to the endpoint
.WithMetadata(new
{
    OpenApi = new OpenApiInfo
    {
        Title = "LocalizationDemo.Api",
        Version = "v1",
        Description = "A simple example of localization in ASP.NET Core 9 minimal APIs",
    }
})
// Add a name to the endpoint
.WithName("GetWeatherForecast")
.WithDescription("Get the weather forecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public partial class Program { } // This is a hack for minimal APIs. We need to add this at the end of the file to expose Program for testing
