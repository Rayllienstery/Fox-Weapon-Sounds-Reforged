#Requires -Version 5.1
<#
.SYNOPSIS
  Install rebuilt Fox audio overlay (full dist/) into SPT 4.0.13 StreamingAssets.
#>
$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent $PSScriptRoot
$AudioRoot = "c:\Games\SPT\EscapeFromTarkov_Data\StreamingAssets\Windows\assets\content\audio"
$DistRoot = Join-Path $RepoRoot "dist\audio"
$BackupRoot = Join-Path $RepoRoot "vanilla-backup"

if (-not (Test-Path (Join-Path $BackupRoot "banks"))) {
  Write-Host "No vanilla-backup found — running backup_vanilla.ps1 first..."
  & (Join-Path $PSScriptRoot "backup_vanilla.ps1")
}

$installed = 0
foreach ($kind in @("banks", "weapons")) {
  $srcDir = Join-Path $DistRoot $kind
  if (-not (Test-Path $srcDir)) { continue }
  Get-ChildItem $srcDir -File -Filter "*.bundle" | ForEach-Object {
    $dest = Join-Path $AudioRoot (Join-Path $kind $_.Name)
    if (-not (Test-Path $dest)) {
      Write-Warning "Skip $($_.Name): no vanilla target at $dest"
      return
    }
    Copy-Item -LiteralPath $_.FullName -Destination $dest -Force
    Write-Host "installed $kind/$($_.Name)"
    $script:installed++
  }
}

if ($installed -eq 0) {
  Write-Warning "Nothing installed. Rebuild bundles into dist/audio/{banks,weapons}/ first."
  exit 1
}
Write-Host "Installed $installed bundle(s). Restart the game client (not only the server)."
