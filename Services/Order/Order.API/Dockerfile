# BASE IMAGE (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 83
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:83
ENV ASPNETCORE_ENVIRONMENT=Development

# BUILD IMAGE
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files
COPY ["Services/Order/Order.API/Order.API.csproj", "Services/Order/Order.API/"]
COPY ["Services/Order/Order.Application/Order.Application.csproj", "Services/Order/Order.Application/"]
COPY ["Services/Order/Order.Infrastructure/Order.Infrastructure.csproj", "Services/Order/Order.Infrastructure/"]
COPY ["RabbitMQInfra/EventBus.Messages/EventBus.Messages.csproj", "RabbitMQInfra/EventBus.Messages/"]
COPY ["Logging/Common.Logging/Common.Logging.csproj", "Logging/Common.Logging/"]


# Restore dependencies
RUN dotnet restore "Services/Order/Order.API/Order.API.csproj"

# Copy the entire solution
COPY . .

# Build the project
WORKDIR "/src/Services/Order/Order.API"
RUN dotnet build "Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# PUBLISH IMAGE
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Order.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FINAL IMAGE
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.API.dll"]
