# Use the official ASP.NET Core 9.0 runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5122
# Set the environment variable for ASP.NET Core to listen on port 5122
ENV ASPNETCORE_URLS=http://+:5122

# Use the official .NET 9.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["LocalizationDemo.Api/LocalizationDemo.Api.csproj", "LocalizationDemo.Api/"]
RUN dotnet restore "LocalizationDemo.Api/LocalizationDemo.Api.csproj"

# Copy the rest of the source code
COPY . .

WORKDIR "/src/LocalizationDemo.Api"
RUN dotnet publish "LocalizationDemo.Api.csproj" -c Release -o /app/publish

# Build runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LocalizationDemo.Api.dll"]