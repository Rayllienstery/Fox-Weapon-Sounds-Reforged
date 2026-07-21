#Requires -Version 5.1
<#
.SYNOPSIS
  Backup vanilla 4.0.13 audio banks/weapons into vanilla-backup/ (full set).
#>
$ErrorActionPreference = "Stop"
$RepoRoot = Split-Path -Parent $PSScriptRoot
$AudioRoot = "c:\Games\SPT\EscapeFromTarkov_Data\StreamingAssets\Windows\assets\content\audio"
$BackupRoot = Join-Path $RepoRoot "vanilla-backup"

foreach ($kind in @("banks", "weapons")) {
  $src = Join-Path $AudioRoot $kind
  $dst = Join-Path $BackupRoot $kind
  New-Item -ItemType Directory -Force -Path $dst | Out-Null
  Get-ChildItem $src -File -Filter "*.bundle" | ForEach-Object {
    Copy-Item -LiteralPath $_.FullName -Destination (Join-Path $dst $_.Name) -Force
    Write-Host "backed up $kind/$($_.Name)"
  }
}
Write-Host "Done. Backup at $BackupRoot"
