
# How to execute
# Operation	Command
# Enable maintenance	.\Deploy-SmartServe.ps1 -MaintenanceMode Enable
# Disable maintenance	.\Deploy-SmartServe.ps1 -MaintenanceMode Disable
# Restart application pool	.\Deploy-SmartServe.ps1 -RestartAppPool
# Enable maintenance + restart	.\Deploy-SmartServe.ps1 -MaintenanceMode Enable -RestartAppPool
# Disable maintenance + restart	.\Deploy-SmartServe.ps1 -MaintenanceMode Disable -RestartAppPool
param(
    [ValidateSet("Enable", "Disable")]
    [string]$MaintenanceMode = "Disable",

    [switch]$RestartAppPool
)

$webRoot = "C:\inetpub\wwwroot\SmartServe-Web"
$maintenanceFlag = Join-Path $webRoot "maintenance.enabled"
$appPool = "SmartServe-Web"

# 1. Set maintenance mode
if ($MaintenanceMode -eq "Enable") {
    New-Item -Path $maintenanceFlag -ItemType File -Force | Out-Null
    Write-Host "Maintenance mode ENABLED." -ForegroundColor Yellow
}
else {
    if (Test-Path $maintenanceFlag) {
        Remove-Item $maintenanceFlag -Force
    }

    Write-Host "Maintenance mode DISABLED." -ForegroundColor Green
}

# 2. Restart application pool if requested
if ($RestartAppPool) {
    Write-Host "Restarting application pool: $appPool..." -ForegroundColor Cyan

    Restart-WebAppPool -Name $appPool

    Start-Sleep -Seconds 3

    $state = (Get-WebAppPoolState -Name $appPool).Value

    Write-Host "Application pool status: $state" -ForegroundColor Green
}

# 3. Verify maintenance flag
$maintenanceStatus = Test-Path $maintenanceFlag

Write-Host "Maintenance flag exists: $maintenanceStatus"
