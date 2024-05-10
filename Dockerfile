FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["RedisCacheWithRateLimitingWebAPI.csproj", "."]
RUN dotnet restore "./././RedisCacheWithRateLimitingWebAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./RedisCacheWithRateLimitingWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./RedisCacheWithRateLimitingWebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 80
EXPOSE 443

ENTRYPOINT ["dotnet", "RedisCacheWithRateLimitingWebAPI.dll"]