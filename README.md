![GitHub last commit](https://img.shields.io/github/last-commit/szblajos/LocalizationDemo)
![GitHub issues](https://img.shields.io/github/issues/szblajos/LocalizationDemo)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

# LocalizationDemo.Api

This project demonstrates how to implement localization in an ASP.NET Core 9 minimal API. The example includes configuring localization services, adding resource files for different cultures, and using a `ResourceLocalizer` generic class to localize weather summaries.
The project based on the current webapi template of .NET 9 SDK, for simplicity.

## Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Running the Project

1. Clone the repository:
   ```bash
   git clone https://github.com/szblajos/LocalizationDemo.git
   cd LocalizationDemo
   ```
2. Build and run the project:
   ```bash
   dotnet build
   dotnet run --project src/LocalizationDemo.Api
   ```
3. Open your browser and navigate to [http://localhost:5122/scalar/]() to explore the API using [Scalar API Client](https://github.com/scalar/scalar)

## Localization
Localization is the process of adapting an application to different languages and cultures. In this project, we use resource files to store localized strings and configure the localization services in the `Program.cs` file.

### Configuring Localization Services
In the `Program.cs` file, we configure localization services and specify the path to the resource files:

```csharp
services.AddLocalization(options => options.ResourcesPath = "Resources");
```
We also configure the supported cultures and set the default culture:

```csharp
services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("hu")
    };

    options.DefaultRequestCulture = new RequestCulture("en-US");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = new[]
    {
        new AcceptLanguageHeaderRequestCultureProvider()
    };
});
```

### Adding Resource Files
Resource files store localized strings for different cultures. In this project, we have two resource files:
- `Resources/WeatherResource.en-US.resx` (English)
- `Resources/WeatherResource.hu-HU.resx` (Hungarian)

Here is an example of the content of `WeatherResource.en-US.resx`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <data name="Freezing" xml:space="preserve">
    <value>Freezing</value>
  </data>
  <data name="Bracing" xml:space="preserve">
    <value>Bracing</value>
  </data>
  <!-- Additional weather summaries -->
</root>
```

### Using the ResourceLocalizer Generic Class
The `ResourceLocalizer` generic class is used to localize strings based on the current culture. We register the `ResourceLocalizer` service in the `Program.cs` file:

```csharp
services.AddScoped<IResourceLocalizer<WeatherResource>, ResourceLocalizer<WeatherResource>>();
```

In the MapGet endpoint, we use the ResourceLocalizer to localize weather summaries:

```csharp
app.MapGet("/weatherforecast", ([FromServices] IResourceLocalizer<WeatherResource> resourceLocalizer) =>
{
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
.WithMetadata(new
{
    OpenApi = new OpenApiInfo
    {
        Title = "LocalizationDemo.Api",
        Version = "v1",
        Description = "A simple example of localization in ASP.NET Core 9 minimal APIs",
    }
})
.WithName("GetWeatherForecast")
.WithDescription("Get the weather forecast");
```

### ResourceLocalizer Class
The `ResourceLocalizer` class is a generic class that provides localization functionality. It uses the `IStringLocalizer` service to retrieve localized strings from resource files.

Here is the implementation of the `ResourceLocalizer` class:

```csharp
public class ResourceLocalizer<T> : IResourceLocalizer<T>
{
    private readonly IStringLocalizer _localizer;
    
    public ResourceLocalizer(IStringLocalizerFactory factory)
    {
        var type = typeof(T);
        var assemblyName = new AssemblyName(type.Assembly.FullName!);
        _localizer = factory.Create(type.Name, assemblyName.Name!);
    }

    // Localize the given key
    public string Localize(string key)
    {
        return _localizer[key];
    }
}
```

The `Localize` method takes a key as a parameter and returns the localized string for the current culture.

### Separate resource files by endpoints
To keep resource files organized, you can separate them by endpoints. For example, if you have multiple endpoints that require localization, you can create separate resource files for each endpoint:

- `Resources/WeatherResource.en-US.resx` (English)
- `Resources/WeatherResource.hu-HU.resx` (Hungarian)
- `Resources/AnotherEndpointResource.en-US.resx` (English)
- `Resources/AnotherEndpointResource.hu-HU.resx` (Hungarian)

This approach helps maintain a clean structure and makes it easier to manage localized strings for different parts of your application.

Place your new endpoint marker class in the Markers.cs file for each endpoints:
```csharp
public class WeatherResource {}
public class AnotherEndpointResource {}  // marker class for a separate endpoint
```

> [!NOTE]
> Note that the marker class name exactly matches the part of the resx file name before the locale information.

Then add a new ResourceLocalizer as a new scoped service, but with the type of the new marker class:
```csharp
services.AddScoped<IResourceLocalizer<AnotherEndpointResource>, ResourceLocalizer<AnotherEndpointResource>>();
```

Inject the `IResourceLocalizer` in your new endpoint:
```csharp
app.MapGet("/anotherendpoint", ([FromServices] IResourceLocalizer<AnotherEndpointResource> resourceLocalizer)
```

By organizing resource files this way, you can easily manage and update localized strings for each endpoint independently.

## Testing Localization with curl

You can test the API's localization by setting the `Accept-Language` header in your requests. Here are some examples:

**English (US):**

```bash
curl -H "Accept-Language: en-US" http://localhost:5122/weatherforecast
```

**Hungarian:**

```bash
curl -H "Accept-Language: hu-HU" http://localhost:5122/weatherforecast
```

**Unsupported language (fallback to en-US):**

```bash
curl -H "Accept-Language: fr-FR" http://localhost:5122/weatherforecast
```

If you use any language not supported by the API, the response will default to English (en-US).

## Conclusion
This project demonstrates how to implement localization in an ASP.NET Core 9 minimal API. By configuring localization services, adding resource files, and using the `ResourceLocalizer` generic class, you can easily adapt your application to different languages and cultures.

## Resources

- [ASP.NET Core Localization Documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization)
- [Resource files in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/work-with-resx-files-programmatically)
- [Localization in .NET 9](https://docs.microsoft.com/en-us/dotnet/core/extensions/localization)
- [Localization in ASP.NET Core: Make Your Minimal APIs Multilingual](https://www.ottorinobruni.com/localization-in-asp-net-core-make-your-minimal-apis-multilingual/)
- [GitHub Repository for LocalizationDemo](https://github.com/szblajos/LocalizationDemo)