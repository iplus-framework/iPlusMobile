#requires -Version 7.0
<#
.SYNOPSIS
    Cleans all obj/bin folders and publishes the ReleaseZebra APK (same as the
    "publish-release-zebra-apk" VS Code task).
#>

$ErrorActionPreference = 'Stop'
$Root = $PSScriptRoot

# 1. Delete all obj and bin folders anywhere under the repo root
Write-Host "Deleting all obj/bin folders under $Root ..." -ForegroundColor Cyan
Get-ChildItem -Path $Root -Recurse -Force -Directory |
    Where-Object { $_.Name -in 'obj', 'bin' } |
    ForEach-Object {
        Write-Host "  Removing $($_.FullName)"
        Remove-Item -Path $_.FullName -Recurse -Force
    }
Write-Host "Clean done." -ForegroundColor Green

# 2. Delete everything inside the publish-zebra output folder
$Output = Join-Path $Root 'publish-zebra'
if (Test-Path $Output) {
    Write-Host "Deleting all contents of $Output ..." -ForegroundColor Cyan
    Get-ChildItem -Path $Output -Force | Remove-Item -Recurse -Force
    Write-Host "Output folder cleaned." -ForegroundColor Green
}

# 3. Publish ReleaseZebra APK (same as tasks.json "publish-release-zebra-apk")
$Project = Join-Path $Root 'gip.vbm.mobileApp\gip.vbm.mobileApp.csproj'

$keystore = $env:MOBILEMAUI_KEYSTORE_PATH
if (-not $keystore) {
    # Fallback to the path used previously on this machine
    $keystore = 'c:\Users\aagincic\My Drive\gipSoft\Key\01_gipSoft\Keystore\mobilemaui\mobilemaui.keystore'
}

Write-Host "Publishing ReleaseZebra APK ..." -ForegroundColor Cyan
& dotnet publish $Project `
    -p:TargetFramework=net10.0-android `
    -p:Configuration=ReleaseZebra `
    -p:AndroidSigningKeyStore="$keystore" `
    -o:$Output

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

Write-Host "Publish succeeded. APK output: $Output" -ForegroundColor Green
