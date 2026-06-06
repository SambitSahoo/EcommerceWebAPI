
# BASE IMAGE (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 81
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:81

# BUILD IMAGE
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Services/Basket/Basket.API/Basket.API.csproj", "Services/Basket/Basket.API/"]
COPY ["Services/Basket/Basket.Application/Basket.Application.csproj", "Services/Basket/Basket.Application/"]
COPY ["Services/Basket/Basket.Core/Basket.Core.csproj", "Services/Basket/Basket.Core/"]
COPY ["Services/Basket/Basket.Infra/Basket.Infra.csproj", "Services/Basket/Basket.Infra/"]
COPY ["RabbitMQInfra/EventBus.Messages/EventBus.Messages.csproj", "RabbitMQInfra/EventBus.Messages/"]
COPY ["Logging/Common.Logging/Common.Logging.csproj", "Logging/Common.Logging/"]


# Restore dependencies
RUN dotnet restore "Services/Basket/Basket.API/Basket.API.csproj"

# Copy the entire solution
COPY . .

# Build the project
WORKDIR "/src/Services/Basket/Basket.API"
RUN dotnet build "Basket.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH IMAGE
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Basket.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL IMAGE
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Basket.API.dll"]
