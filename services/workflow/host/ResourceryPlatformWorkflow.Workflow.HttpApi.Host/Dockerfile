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
COPY ["services/projects/host/ResourceryPlatformWorkflow.Workflow.HttpApi.Host/ResourceryPlatformWorkflow.Workflow.HttpApi.Host.csproj", "services/projects/host/ResourceryPlatformWorkflow.Workflow.HttpApi.Host/"]
COPY ["shared/Resourcery.Microservice.Shared/Resourcery.Microservice.Shared.csproj", "shared/Resourcery.Microservice.Shared/"]
COPY ["services/administration/src/Resourcery.Administration.EntityFrameworkCore/Resourcery.Administration.EntityFrameworkCore.csproj", "services/administration/src/Resourcery.Administration.EntityFrameworkCore/"]
COPY ["shared/Resourcery.Shared/Resourcery.Shared.csproj", "shared/Resourcery.Shared/"]
COPY ["services/administration/src/Resourcery.Administration.Domain/Resourcery.Administration.Domain.csproj", "services/administration/src/Resourcery.Administration.Domain/"]
COPY ["services/administration/src/Resourcery.Administration.Domain.Shared/Resourcery.Administration.Domain.Shared.csproj", "services/administration/src/Resourcery.Administration.Domain.Shared/"]
COPY ["shared/Resourcery.Hosting.Shared/Resourcery.Hosting.Shared.csproj", "shared/Resourcery.Hosting.Shared/"]
COPY ["shared/Resourcery.ServiceDefaults/Resourcery.ServiceDefaults.csproj", "shared/Resourcery.ServiceDefaults/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Workflow.HttpApi/ResourceryPlatformWorkflow.Workflow.HttpApi.csproj", "services/projects/src/ResourceryPlatformWorkflow.Workflow.HttpApi/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Workflow.Application.Contracts/ResourceryPlatformWorkflow.Workflow.Application.Contracts.csproj", "services/projects/src/ResourceryPlatformWorkflow.Workflow.Application.Contracts/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/ResourceryPlatformWorkflow.Workflow.Domain.Shared.csproj", "services/projects/src/ResourceryPlatformWorkflow.Workflow.Domain.Shared/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Workflow.Application/ResourceryPlatformWorkflow.Workflow.Application.csproj", "services/projects/src/ResourceryPlatformWorkflow.Workflow.Application/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Workflow.Domain/ResourceryPlatformWorkflow.Workflow.Domain.csproj", "services/projects/src/ResourceryPlatformWorkflow.Workflow.Domain/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore/ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore.csproj", "services/projects/src/ResourceryPlatformWorkflow.Workflow.EntityFrameworkCore/"]
RUN dotnet restore "./services/projects/host/ResourceryPlatformWorkflow.Workflow.HttpApi.Host/ResourceryPlatformWorkflow.Workflow.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/services/projects/host/ResourceryPlatformWorkflow.Workflow.HttpApi.Host"
RUN dotnet build "./ResourceryPlatformWorkflow.Workflow.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ResourceryPlatformWorkflow.Workflow.HttpApi.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResourceryPlatformWorkflow.Workflow.HttpApi.Host.dll"]