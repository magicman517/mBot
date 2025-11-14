FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Services/Bot/Bot.Gateway/Bot.Gateway.csproj", "src/Services/Bot/Bot.Gateway/"]
RUN dotnet restore "src/Services/Bot/Bot.Gateway/Bot.Gateway.csproj"
COPY . .
WORKDIR "/src/src/Services/Bot/Bot.Gateway"
RUN dotnet build "./Bot.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bot.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bot.Gateway.dll"]
