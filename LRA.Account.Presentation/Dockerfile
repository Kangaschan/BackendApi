FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Debug 
WORKDIR /src
COPY ["LRA.Account.Presentation/LRA.Account.Presentation.csproj", "LRA.Account.Presentation/"]
COPY ["LRA.Account.Application/LRA.Account.Application.csproj", "LRA.Account.Application/"]
COPY ["LRA.Infrastructure.Messaging/LRA.Infrastructure.Messaging.csproj", "LRA.Infrastructure.Messaging/"]
COPY ["LRA.Account.DBInfrastructure/LRA.Account.DBInfrastructure.csproj", "LRA.Account.DBInfrastructure/"]
COPY ["LRA.Account.KeycloakInfrastructure/LRA.Account.KeycloakInfrastructure.csproj", "LRA.Account.KeycloakInfrastructure/"]
COPY ["LRA.Account.AzureInfrastructure/LRA.Account.AzureInfrastructure.csproj", "LRA.Account.AzureInfrastructure/"]
COPY ["LRA.Account.Domain/LRA.Account.Domain.csproj", "LRA.Account.Domain/"]
COPY ["LRA.Common/LRA.Common.csproj", "LRA.Common/"]

RUN dotnet restore "LRA.Account.Presentation/LRA.Account.Presentation.csproj"
COPY . .
WORKDIR "/src/LRA.Account.Presentation"
COPY ["LRA.Account.DBInfrastructure/Migrations/*", "app/Migrations/"]
RUN dotnet build "LRA.Account.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug 
RUN dotnet publish "LRA.Account.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/LRA.Account.Presentation/appsettings*.json .
ENTRYPOINT ["dotnet", "LRA.Account.Presentation.dll"]
