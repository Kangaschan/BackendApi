FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LRA.Gateways.Client/LRA.Gateways.Client.csproj", "LRA.Gateways.Client/"]
RUN dotnet restore "LRA.Gateways.Client/LRA.Gateways.Client.csproj"
COPY . .
WORKDIR "/src/LRA.Gateways.Client"
RUN dotnet build "LRA.Gateways.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LRA.Gateways.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/LRA.Gateways.Client/appsettings*.json .
ENTRYPOINT ["dotnet", "LRA.Gateways.Client.dll"]
