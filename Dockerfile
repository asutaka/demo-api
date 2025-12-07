# --- Stage 1: build ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy csproj và restore dependencies
COPY ["DemoApi.csproj", "./"]
RUN dotnet restore "./DemoApi.csproj"

# 2. Copy toàn bộ source và build
COPY . .
RUN dotnet build "./DemoApi.csproj" -c Release -o /app/build

# 3. Publish (release) để output sạch, tối ưu
RUN dotnet publish "./DemoApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# --- Stage 2: runtime ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
# Port mà ASP.NET Core trong .NET 8 thường listen
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Copy output từ stage publish
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DemoApi.dll"]
