FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8086

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LRA.Gateways.Admin/LRA.Gateways.Admin.csproj", "LRA.Gateways.Admin/"]
COPY ["LRA.Common/LRA.Common.csproj", "LRA.Common/"]
RUN dotnet restore "LRA.Gateways.Admin/LRA.Gateways.Admin.csproj"
COPY . .
WORKDIR "/src/LRA.Gateways.Admin"
RUN dotnet build "LRA.Gateways.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LRA.Gateways.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/LRA.Gateways.Admin/appsettings*.json .
ENTRYPOINT ["dotnet", "LRA.Gateways.Admin.dll"]
