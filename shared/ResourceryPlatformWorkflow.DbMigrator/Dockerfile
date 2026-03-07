# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["services/administration/NuGet.Config", "services/administration/"]
COPY ["services/identity/NuGet.Config", "services/identity/"]
COPY ["services/projects/NuGet.Config", "services/projects/"]
COPY ["services/saas/NuGet.Config", "services/saas/"]
COPY ["shared/ResourceryPlatformWorkflow.DbMigrator/ResourceryPlatformWorkflow.DbMigrator.csproj", "shared/ResourceryPlatformWorkflow.DbMigrator/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.Application.Contracts/ResourceryPlatformWorkflow.Administration.Application.Contracts.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.Application.Contracts/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.Domain.Shared/ResourceryPlatformWorkflow.Administration.Domain.Shared.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.Domain.Shared/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.EntityFrameworkCore/"]
COPY ["shared/ResourceryPlatformWorkflow.Shared/ResourceryPlatformWorkflow.Shared.csproj", "shared/ResourceryPlatformWorkflow.Shared/"]
COPY ["services/administration/src/ResourceryPlatformWorkflow.Administration.Domain/ResourceryPlatformWorkflow.Administration.Domain.csproj", "services/administration/src/ResourceryPlatformWorkflow.Administration.Domain/"]
COPY ["services/identity/src/ResourceryPlatformWorkflow.IdentityService.Application.Contracts/ResourceryPlatformWorkflow.IdentityService.Application.Contracts.csproj", "services/identity/src/ResourceryPlatformWorkflow.IdentityService.Application.Contracts/"]
COPY ["services/identity/src/ResourceryPlatformWorkflow.IdentityService.Domain.Shared/ResourceryPlatformWorkflow.IdentityService.Domain.Shared.csproj", "services/identity/src/ResourceryPlatformWorkflow.IdentityService.Domain.Shared/"]
COPY ["services/identity/src/ResourceryPlatformWorkflow.IdentityService.EntityFrameworkCore/ResourceryPlatformWorkflow.IdentityService.EntityFrameworkCore.csproj", "services/identity/src/ResourceryPlatformWorkflow.IdentityService.EntityFrameworkCore/"]
COPY ["services/identity/src/ResourceryPlatformWorkflow.IdentityService.Domain/ResourceryPlatformWorkflow.IdentityService.Domain.csproj", "services/identity/src/ResourceryPlatformWorkflow.IdentityService.Domain/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Projects.Application.Contracts/ResourceryPlatformWorkflow.Projects.Application.Contracts.csproj", "services/projects/src/ResourceryPlatformWorkflow.Projects.Application.Contracts/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Projects.Domain.Shared/ResourceryPlatformWorkflow.Projects.Domain.Shared.csproj", "services/projects/src/ResourceryPlatformWorkflow.Projects.Domain.Shared/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Projects.EntityFrameworkCore/ResourceryPlatformWorkflow.Projects.EntityFrameworkCore.csproj", "services/projects/src/ResourceryPlatformWorkflow.Projects.EntityFrameworkCore/"]
COPY ["services/projects/src/ResourceryPlatformWorkflow.Projects.Domain/ResourceryPlatformWorkflow.Projects.Domain.csproj", "services/projects/src/ResourceryPlatformWorkflow.Projects.Domain/"]
COPY ["services/saas/src/ResourceryPlatformWorkflow.SaaS.Application.Contracts/ResourceryPlatformWorkflow.SaaS.Application.Contracts.csproj", "services/saas/src/ResourceryPlatformWorkflow.SaaS.Application.Contracts/"]
COPY ["services/saas/src/ResourceryPlatformWorkflow.SaaS.Domain.Shared/ResourceryPlatformWorkflow.SaaS.Domain.Shared.csproj", "services/saas/src/ResourceryPlatformWorkflow.SaaS.Domain.Shared/"]
COPY ["services/saas/src/ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore/ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore.csproj", "services/saas/src/ResourceryPlatformWorkflow.SaaS.EntityFrameworkCore/"]
COPY ["services/saas/src/ResourceryPlatformWorkflow.SaaS.Domain/ResourceryPlatformWorkflow.SaaS.Domain.csproj", "services/saas/src/ResourceryPlatformWorkflow.SaaS.Domain/"]
COPY ["shared/ResourceryPlatformWorkflow.Microservice.Shared/ResourceryPlatformWorkflow.Microservice.Shared.csproj", "shared/ResourceryPlatformWorkflow.Microservice.Shared/"]
COPY ["shared/ResourceryPlatformWorkflow.Hosting.Shared/ResourceryPlatformWorkflow.Hosting.Shared.csproj", "shared/ResourceryPlatformWorkflow.Hosting.Shared/"]
COPY ["shared/ResourceryPlatformWorkflow.ServiceDefaults/ResourceryPlatformWorkflow.ServiceDefaults.csproj", "shared/ResourceryPlatformWorkflow.ServiceDefaults/"]
RUN dotnet restore "./shared/ResourceryPlatformWorkflow.DbMigrator/ResourceryPlatformWorkflow.DbMigrator.csproj"
COPY . .
WORKDIR "/src/shared/ResourceryPlatformWorkflow.DbMigrator"
RUN dotnet build "./ResourceryPlatformWorkflow.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ResourceryPlatformWorkflow.DbMigrator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ResourceryPlatformWorkflow.DbMigrator.dll"]