FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["CarbonTrackerApi/CarbonTrackerApi.csproj", "CarbonTrackerApi/"]

RUN dotnet restore "CarbonTrackerApi/CarbonTrackerApi.csproj"

COPY . .


FROM build AS publish
RUN dotnet publish "CarbonTrackerApi/CarbonTrackerApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "CarbonTrackerApi.dll"]
