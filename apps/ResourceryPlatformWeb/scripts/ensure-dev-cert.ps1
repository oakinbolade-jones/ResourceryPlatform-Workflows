$ErrorActionPreference = 'Stop'

$projectRoot = Split-Path -Parent $PSScriptRoot
$certDirectory = Join-Path $projectRoot '.cert'
$certPath = Join-Path $certDirectory 'localhost.pem'
$keyPath = Join-Path $certDirectory 'localhost.key'

if (-not (Test-Path $certDirectory)) {
    New-Item -ItemType Directory -Path $certDirectory | Out-Null
}

dotnet dev-certs https --trust | Out-Null
dotnet dev-certs https --export-path $certPath --format Pem --no-password | Out-Null

if (-not (Test-Path $certPath) -or -not (Test-Path $keyPath)) {
    throw 'Failed to export the development HTTPS certificate for Angular.'
}