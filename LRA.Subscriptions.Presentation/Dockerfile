FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8084

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LRA.Subscriptions.Presentation/LRA.Subscriptions.Presentation.csproj", "LRA.Subscriptions.Presentation/"]
COPY ["LRA.Subscriptions.Application/LRA.Subscriptions.Application.csproj", "LRA.Subscriptions.Application/"]
COPY ["LRA.Subscriptions.StripeInfrastructure/LRA.Subscriptions.StripeInfrastructure.csproj", "LRA.Subscriptions.StripeInfrastructure/"]
COPY ["LRA.Subscriptions.DBInfrastructure/LRA.Subscriptions.DBInfrastructure.csproj", "LRA.Subscriptions.DBInfrastructure/"]
COPY ["LRA.Subscriptions.Domain/LRA.Subscriptions.Domain.csproj", "LRA.Subscriptions.Domain/"]
COPY ["LRA.Common/LRA.Common.csproj", "LRA.Common/"]
RUN dotnet restore "LRA.Subscriptions.Presentation/LRA.Subscriptions.Presentation.csproj"
COPY . .
WORKDIR "/src/LRA.Subscriptions.Presentation"
RUN dotnet build "LRA.Subscriptions.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LRA.Subscriptions.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /src/LRA.Subscriptions.Presentation/appsettings*.json .
ENTRYPOINT ["dotnet", "LRA.Subscriptions.Presentation.dll"]
