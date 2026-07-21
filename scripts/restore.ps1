#Requires -Version 5.1
<#
.SYNOPSIS
  Restore vanilla audio banks/weapons from vanilla-backup/.
#>
$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent $PSScriptRoot
$AudioRoot = "c:\Games\SPT\EscapeFromTarkov_Data\StreamingAssets\Windows\assets\content\audio"
$BackupRoot = Join-Path $RepoRoot "vanilla-backup"

$restored = 0
foreach ($kind in @("banks", "weapons")) {
  $srcDir = Join-Path $BackupRoot $kind
  if (-not (Test-Path $srcDir)) {
    throw "Missing backup: $srcDir — run scripts/backup_vanilla.ps1 first."
  }
  Get-ChildItem $srcDir -File -Filter "*.bundle" | ForEach-Object {
    $dest = Join-Path $AudioRoot (Join-Path $kind $_.Name)
    Copy-Item -LiteralPath $_.FullName -Destination $dest -Force
    Write-Host "restored $kind/$($_.Name)"
    $script:restored++
  }
}
Write-Host "Restored $restored bundle(s). Restart the game client."
