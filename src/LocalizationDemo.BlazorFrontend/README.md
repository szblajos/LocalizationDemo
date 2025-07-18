# LocalizationDemo.BlazorFrontend

A Blazor WebAssembly frontend application that demonstrates internationalization (i18n) and localization (l10n) by integrating with the LocalizationDemo.Api backend.

## Features

- **Language Selector**: Dropdown in the header to switch between English (en-US) and Hungarian (hu) locales
- **Real-time Localization**: Weather data updates immediately when language is changed
- **API Integration**: Fetches localized weather forecasts from LocalizationDemo.Api
- **State Persistence**: Selected language is saved to localStorage
- **Automatic Headers**: HTTP requests automatically include appropriate Accept-Language headers

## Prerequisites

- .NET 9.0 SDK
- LocalizationDemo.Api running on `http://localhost:5122`

## Local Development Setup

### 1. Start the API Backend

```bash
cd src/LocalizationDemo.Api
dotnet run
```

The API should be running on `http://localhost:5122`

### 2. Start the Blazor Frontend

```bash
cd src/LocalizationDemo.BlazorFrontend
dotnet run
```

The frontend will be available at `http://localhost:5254`

### 3. Test the Application

1. Navigate to `http://localhost:5254`
2. Click on the "Weather" link in the navigation
3. Observe the weather data in English
4. Click on the "English ▼" dropdown in the top-right corner
5. Select "Magyar" to switch to Hungarian
6. Notice the weather summaries change to Hungarian terms (Hűvös, Meleg, etc.)

## Architecture

### Components

- **LanguageSelector**: Dropdown component for language selection
- **LanguageService**: Service for managing language state and HTTP client configuration
- **Weather Page**: Displays localized weather data from the API

### Key Files

- `Services/LanguageService.cs`: Language state management and HTTP client configuration
- `Components/LanguageSelector.razor`: Language dropdown UI component  
- `Pages/Weather.razor`: Weather forecast page with API integration
- `Program.cs`: DI configuration and HTTP client setup

## Configuration

The frontend is configured to connect to the API at `http://localhost:5122` by default. This can be modified in `Program.cs`:

```csharp
var apiBaseAddress = builder.Configuration["ApiBaseAddress"] ?? "http://localhost:5122";
```

You can override this by setting the `ApiBaseAddress` configuration value.