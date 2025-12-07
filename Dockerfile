# ============================
#       BUILD STAGE
# ============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY DemoSolution.sln ./

# Copy project
COPY DemoApi/DemoApi.csproj DemoApi/

# Restore dependencies
RUN dotnet restore DemoApi/DemoApi.csproj

# Copy toàn bộ source
COPY . .

# Build
WORKDIR /src/DemoApi
RUN dotnet build -c Release -o /app/build

# ============================
#       PUBLISH STAGE
# ============================
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# ============================
#       RUNTIME STAGE
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Cài package thiếu
RUN apt-get update && apt-get install -y libgssapi-krb5-2

WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "DemoApi.dll"]
