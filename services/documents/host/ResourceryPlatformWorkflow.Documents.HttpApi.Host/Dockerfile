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
COPY ["services/projects/host/ResourceryPlatformWorkflow.Documents.HttpApi.Host/ResourceryPlatformWorkflow.Documents.HttpApi.Host.csproj", "services/projects/host/ResourceryPlatformWorkflow.Documents.HttpApi.Host/"]
COPY ["shared/ResourceryPlatformWorkflow.Microservice.Shared/ResourceryPlatformWorkflow.Microservice.Shared.csproj", "shared/ResourceryPlatformWorkflow.Microservice.Shared/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore/"]
COPY ["shared/ResourceryPlatformWorkflow.Shared/ResourceryPlatformWorkflow.Shared.csproj", "shared/ResourceryPlatformWorkflow.Shared/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.Domain/ResourceryPlatformWorkflow.Administration.Domain.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.Domain/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.Domain.Shared/ResourceryPlatformWorkflow.Administration.Domain.Shared.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.Domain.Shared/"]
COPY ["shared/ResourceryPlatformWorkflow.Hosting.Shared/ResourceryPlatformWorkflow.Hosting.Shared.csproj", "shared/ResourceryPlatformWorkflow.Hosting.Shared/"]
COPY ["shared/ResourceryPlatformWorkflow.ServiceDefaults/ResourceryPlatformWorkflow.ServiceDefaults.csproj", "shared/ResourceryPlatformWorkflow.ServiceDefaults/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Documents.HttpApi/ResourceryPlatformWorkflow.Documents.HttpApi.csproj", "services/projects/src/ResourceryPlatformWorkflow.Documents.HttpApi/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Documents.Application.Contracts/ResourceryPlatformWorkflow.Documents.Application.Contracts.csproj", "services/projects/src/ResourceryPlatformWorkflow.Documents.Application.Contracts/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Documents.Domain.Shared/ResourceryPlatformWorkflow.Documents.Domain.Shared.csproj", "services/projects/src/ResourceryPlatformWorkflow.Documents.Domain.Shared/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Documents.Application/ResourceryPlatformWorkflow.Documents.Application.csproj", "services/projects/src/ResourceryPlatformWorkflow.Documents.Application/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Documents.Domain/ResourceryPlatformWorkflow.Documents.Domain.csproj", "services/projects/src/ResourceryPlatformWorkflow.Documents.Domain/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Documents.EntityFrameworkCore/ResourceryPlatformWorkflow.Documents.EntityFrameworkCore.csproj", "services/projects/src/ResourceryPlatformWorkflow.Documents.EntityFrameworkCore/"]
RUN dotnet restore "./services/projects/host/ResourceryPlatformWorkflow.Documents.HttpApi.Host/ResourceryPlatformWorkflow.Documents.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/services/projects/host/ResourceryPlatformWorkflow.Documents.HttpApi.Host"
RUN dotnet build "./ResourceryPlatformWorkflow.Documents.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ResourceryPlatformWorkflow.Documents.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResourceryPlatformWorkflow.Documents.HttpApi.Host.dll"]