# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY DemoApi.sln ./

# Copy project folder
COPY DemoApi ./DemoApi

# Restore dependencies
RUN dotnet restore

# Build the project
RUN dotnet build DemoApi/DemoApi.csproj -c Release -o /app/build

# Publish
RUN dotnet publish DemoApi/DemoApi.csproj -c Release -o /app/publish /p:UseAppHost=false

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DemoApi.dll"]
