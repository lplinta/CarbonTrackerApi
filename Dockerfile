# Build e Test
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CarbonTrackerApi/CarbonTrackerApi.csproj", "CarbonTrackerApi/"]
COPY ["CarbonTrackerApi.UnitTests/CarbonTrackerApi.UnitTests.csproj", "CarbonTrackerApi.UnitTests/"]

RUN dotnet restore "CarbonTrackerApi/CarbonTrackerApi.csproj"
RUN dotnet restore "CarbonTrackerApi.UnitTests/CarbonTrackerApi.UnitTests.csproj"

COPY . .

RUN dotnet build "CarbonTrackerApi/CarbonTrackerApi.csproj" -c Release -o /app/build

RUN dotnet test "CarbonTrackerApi.UnitTests/CarbonTrackerApi.UnitTests.csproj" --no-build --verbosity normal

# Publish
FROM build AS publish
RUN dotnet publish "CarbonTrackerApi/CarbonTrackerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CarbonTrackerApi.dll"]
