param(
    [switch]$SkipNetworkCleanup = $false,
    [switch]$SkipRun = $false
)

$ErrorActionPreference = "Stop"

function Write-Step {
    param([string]$Message)
    Write-Host "[AppHost Cleanup] $Message" -ForegroundColor Cyan
}

function Remove-StaleAppHostProcesses {
    Write-Step "Stopping stale AppHost and DCP processes (if any)."

    $names = @("dcp", "ResourceryPlatformWorkflow.AppHost")

    foreach ($name in $names) {
        $procs = Get-Process -Name $name -ErrorAction SilentlyContinue
        if ($null -ne $procs) {
            $procs | Stop-Process -Force -ErrorAction SilentlyContinue
            Write-Host "  Stopped process '$name'."
        }
    }
}

function Remove-StaleContainers {
    Write-Step "Removing stale Created/Exited infrastructure containers."

    $namePattern = '^(rabbitmq|redis|seq|rediscommander|redis-commander)-'
    $all = docker ps -a --format "{{.ID}}`t{{.Names}}`t{{.Status}}"

    if ([string]::IsNullOrWhiteSpace($all)) {
        Write-Host "  No containers found."
        return
    }

    $toRemove = @()

    foreach ($line in $all) {
        $parts = $line -split "`t", 3
        if ($parts.Count -lt 3) { continue }

        $id = $parts[0]
        $name = $parts[1]
        $status = $parts[2]

        $isInfraName = $name -match $namePattern
        $isStaleStatus = $status.StartsWith("Created") -or $status.StartsWith("Exited")

        if ($isInfraName -and $isStaleStatus) {
            $toRemove += $id
            Write-Host "  Marked '$name' ($status) for removal."
        }
    }

    if ($toRemove.Count -eq 0) {
        Write-Host "  No stale infrastructure containers found."
        return
    }

    foreach ($id in $toRemove) {
        docker rm $id | Out-Null
    }

    Write-Host "  Removed $($toRemove.Count) stale container(s)."
}

function Remove-UnusedAspireNetworks {
    if ($SkipNetworkCleanup) {
        Write-Step "Skipping network cleanup by request."
        return
    }

    Write-Step "Removing unused Aspire-style networks."

    $networkRows = docker network ls --format "{{.ID}}`t{{.Name}}"

    if ([string]::IsNullOrWhiteSpace($networkRows)) {
        Write-Host "  No Docker networks found."
        return
    }

    $removed = 0

    foreach ($row in $networkRows) {
        $parts = $row -split "`t", 2
        if ($parts.Count -lt 2) { continue }

        $networkId = $parts[0]
        $networkName = $parts[1]

        if ($networkName -notmatch '^(aspire|dcp)-') {
            continue
        }

        $inspectJson = docker network inspect $networkId --format "{{json .Containers}}"

        $hasContainers = $false
        if (-not [string]::IsNullOrWhiteSpace($inspectJson) -and $inspectJson -ne "null" -and $inspectJson -ne "{}") {
            $hasContainers = $true
        }

        if ($hasContainers) {
            continue
        }

        docker network rm $networkId | Out-Null
        Write-Host "  Removed unused network '$networkName'."
        $removed++
    }

    if ($removed -eq 0) {
        Write-Host "  No unused Aspire-style networks found."
    }
}

function Start-AppHost {
    if ($SkipRun) {
        Write-Step "Skipping dotnet run by request."
        return
    }

    Write-Step "Starting AppHost."
    dotnet run
}

try {
    Remove-StaleAppHostProcesses
    Remove-StaleContainers
    Remove-UnusedAspireNetworks
    Start-AppHost
}
catch {
    Write-Host "[AppHost Cleanup] Failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
