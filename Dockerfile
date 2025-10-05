FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["api2025/api2025.csproj", "api2025/"]
RUN dotnet restore "api2025/api2025.csproj"
COPY . .
WORKDIR "/src/api2025"
RUN dotnet build "api2025.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "api2025.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p /app/wwwroot/reports && chmod -R 777 /app/wwwroot/reports
RUN apt-get install -y libfontconfig1 libfreetype6 libharfbuzz0b


ENTRYPOINT ["dotnet", "api2025.dll"]
