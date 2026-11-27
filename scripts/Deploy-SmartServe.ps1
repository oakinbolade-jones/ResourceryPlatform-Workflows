# How to execute
# Operation                  Command
# Enable maintenance         .\Deploy-SmartServe.ps1 -MaintenanceMode Enable
# Disable maintenance        .\Deploy-SmartServe.ps1 -MaintenanceMode Disable
# Enable launch countdown    .\Deploy-SmartServe.ps1 -MaintenanceMode Launch
# Restart application pool   .\Deploy-SmartServe.ps1 -RestartAppPool
# Enable maintenance + restart  .\Deploy-SmartServe.ps1 -MaintenanceMode Enable -RestartAppPool
# Disable maintenance + restart .\Deploy-SmartServe.ps1 -MaintenanceMode Disable -RestartAppPool
# Enable launch + restart        .\Deploy-SmartServe.ps1 -MaintenanceMode Launch -RestartAppPool
param(
    [ValidateSet("Enable", "Disable", "Launch")]
    [string]$MaintenanceMode = "Disable",

    [switch]$RestartAppPool
)

$webRoot = "C:\inetpub\wwwroot\SmartServe-Web"
$maintenanceFlag = Join-Path $webRoot "maintenance.enabled"
$launchFlag = Join-Path $webRoot "launch.enabled"
$appPool = "SmartServe-Web"

# 1. Set maintenance mode
switch ($MaintenanceMode) {
    "Enable" {
        if (Test-Path $launchFlag) {
            Remove-Item $launchFlag -Force
        }

        New-Item -Path $maintenanceFlag -ItemType File -Force | Out-Null
        Write-Host "Maintenance mode ENABLED." -ForegroundColor Yellow
    }

    "Launch" {
        if (Test-Path $maintenanceFlag) {
            Remove-Item $maintenanceFlag -Force
        }

        New-Item -Path $launchFlag -ItemType File -Force | Out-Null
        Write-Host "Launch countdown mode ENABLED." -ForegroundColor Yellow
    }

    "Disable" {
        foreach ($flag in @($maintenanceFlag, $launchFlag)) {
            if (Test-Path $flag) {
                Remove-Item $flag -Force
            }
        }

        Write-Host "Maintenance and launch countdown modes DISABLED." -ForegroundColor Green
    }
}

# 2. Restart application pool if requested
if ($RestartAppPool) {
    Write-Host "Restarting application pool: $appPool..." -ForegroundColor Cyan

    Restart-WebAppPool -Name $appPool

    Start-Sleep -Seconds 3

    $state = (Get-WebAppPoolState -Name $appPool).Value

    Write-Host "Application pool status: $state" -ForegroundColor Green
}

# 3. Verify mode flags
$maintenanceStatus = Test-Path $maintenanceFlag
$launchStatus = Test-Path $launchFlag

Write-Host "Maintenance flag exists: $maintenanceStatus"
Write-Host "Launch flag exists: $launchStatus"
