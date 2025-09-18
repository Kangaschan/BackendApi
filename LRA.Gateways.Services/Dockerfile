FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8085

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LRA.Gateways.Services/LRA.Gateways.Services.csproj", "LRA.Gateways.Services/"]
COPY ["LRA.Infrastructure.Messaging/LRA.Infrastructure.Messaging.csproj", "LRA.Infrastructure.Messaging/"]
COPY ["LRA.Common/LRA.Common.csproj", "LRA.Common/"]
RUN dotnet restore "LRA.Gateways.Services/LRA.Gateways.Services.csproj"
COPY . .
WORKDIR "/src/LRA.Gateways.Services"
RUN dotnet build "LRA.Gateways.Services.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LRA.Gateways.Services.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/LRA.Gateways.Services/appsettings*.json .
ENTRYPOINT ["dotnet", "LRA.Gateways.Services.dll"]
