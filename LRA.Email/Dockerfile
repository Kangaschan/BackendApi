FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["LRA.Email/LRA.Email.csproj", "LRA.Email/"]
COPY ["LRA.Common/LRA.Common.csproj", "LRA.Common/"]
COPY ["LRA.Infrastructure.Messaging/LRA.Infrastructure.Messaging.csproj", "LRA.Infrastructure.Messaging/"]
RUN dotnet restore "LRA.Email/LRA.Email.csproj"
COPY . .
WORKDIR "/src/LRA.Email"
RUN dotnet build "LRA.Email.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LRA.Email.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LRA.Email.dll"]
