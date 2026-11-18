# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/projects/NuGet.Config", "services/projects/"]
COPY ["services/administration/NuGet.Config", "services/administration/"]
COPY ["services/projects/host/ResourceryPlatformWorkflow.Mailing.HttpApi.Host/ResourceryPlatformWorkflow.Mailing.HttpApi.Host.csproj", "services/projects/host/ResourceryPlatformWorkflow.Mailing.HttpApi.Host/"]
COPY ["shared/ResourceryPlatformWorkflow.Microservice.Shared/ResourceryPlatformWorkflow.Microservice.Shared.csproj", "shared/ResourceryPlatformWorkflow.Microservice.Shared/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore/"]
COPY ["shared/ResourceryPlatformWorkflow.Shared/ResourceryPlatformWorkflow.Shared.csproj", "shared/ResourceryPlatformWorkflow.Shared/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.Domain/ResourceryPlatformWorkflow.Administration.Domain.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.Domain/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.Domain.Shared/ResourceryPlatformWorkflow.Administration.Domain.Shared.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.Domain.Shared/"]
COPY ["shared/ResourceryPlatformWorkflow.Hosting.Shared/ResourceryPlatformWorkflow.Hosting.Shared.csproj", "shared/ResourceryPlatformWorkflow.Hosting.Shared/"]
COPY ["shared/ResourceryPlatformWorkflow.ServiceDefaults/ResourceryPlatformWorkflow.ServiceDefaults.csproj", "shared/ResourceryPlatformWorkflow.ServiceDefaults/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Mailing.HttpApi/ResourceryPlatformWorkflow.Mailing.HttpApi.csproj", "services/projects/src/ResourceryPlatformWorkflow.Mailing.HttpApi/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Mailing.Application.Contracts/ResourceryPlatformWorkflow.Mailing.Application.Contracts.csproj", "services/projects/src/ResourceryPlatformWorkflow.Mailing.Application.Contracts/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Mailing.Domain.Shared/ResourceryPlatformWorkflow.Mailing.Domain.Shared.csproj", "services/projects/src/ResourceryPlatformWorkflow.Mailing.Domain.Shared/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Mailing.Application/ResourceryPlatformWorkflow.Mailing.Application.csproj", "services/projects/src/ResourceryPlatformWorkflow.Mailing.Application/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Mailing.Domain/ResourceryPlatformWorkflow.Mailing.Domain.csproj", "services/projects/src/ResourceryPlatformWorkflow.Mailing.Domain/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Mailing.EntityFrameworkCore/ResourceryPlatformWorkflow.Mailing.EntityFrameworkCore.csproj", "services/projects/src/ResourceryPlatformWorkflow.Mailing.EntityFrameworkCore/"]
RUN dotnet restore "./services/projects/host/ResourceryPlatformWorkflow.Mailing.HttpApi.Host/ResourceryPlatformWorkflow.Mailing.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/services/projects/host/ResourceryPlatformWorkflow.Mailing.HttpApi.Host"
RUN dotnet build "./ResourceryPlatformWorkflow.Mailing.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ResourceryPlatformWorkflow.Mailing.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResourceryPlatformWorkflow.Mailing.HttpApi.Host.dll"]