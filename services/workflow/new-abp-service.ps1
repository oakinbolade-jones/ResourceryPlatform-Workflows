#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Creates a new ABP microservice with the correct directory structure.

.DESCRIPTION
    Wrapper script for dotnet new abp-microservice-addservice that automatically
    creates the output directory based on the service name.

.PARAMETER Name
    The full service name in format 'CompanyName.ServiceName' (e.g., ResourceryHR.Recruitment)

.EXAMPLE
    .\new-abp-service.ps1 ResourceryHR.Recruitment
    Creates a 'recruitment' directory with the service structure.
#>

param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$Name,
    
    [Parameter(Mandatory=$false)]
    [switch]$Force
)

# Validate the name format
if ($Name -notmatch '^[A-Z][a-zA-Z0-9]*\.[A-Z][a-zA-Z0-9]*$') {
    Write-Error "The name must be in format 'CompanyName.ServiceName' (e.g., ResourceryHR.Recruitment)"
    exit 1
}

# Extract the service name (part after the dot)
if ($Name -match '\.([^.]+)$') {
    $serviceName = $matches[1].ToLower()
} else {
    Write-Error "Could not extract service name from '$Name'"
    exit 1
}

# Create the service
Write-Host "Creating ABP service '$Name' in directory '$serviceName'..." -ForegroundColor Green

$forceArg = if ($Force) { "--force" } else { "" }
$command = "dotnet new abp-microservice-addservice -n $Name -o $serviceName $forceArg".Trim()
Invoke-Expression $command

if ($LASTEXITCODE -eq 0) {
    Write-Host "Service created successfully in '$serviceName' directory!" -ForegroundColor Green
    Write-Host "Workflow: $Name.Application, $Name.Domain, etc." -ForegroundColor Cyan
}
