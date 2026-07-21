#Requires -Version 5.1
$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$SptRoot = "c:\Games\SPT"
$Upstream = Join-Path $RepoRoot "_upstream\FoxSoundMod 3.0.0"
$Proj = Join-Path $RepoRoot "FoxWeaponSoundsReforged\FoxWeaponSoundsReforged.csproj"
$ModDir = Join-Path $SptRoot "SPT\user\mods\Fox-Weapon-Sounds-Reforged"

if (-not (Test-Path -LiteralPath $Upstream)) {
  throw "Missing upstream at $Upstream"
}

Write-Host "Building..."
dotnet build $Proj -c Release
if ($LASTEXITCODE -ne 0) { throw "dotnet build failed" }

$dllSrc = Join-Path $RepoRoot "FoxWeaponSoundsReforged\bin\Release\Fox-Weapon-Sounds-Reforged\Fox-Weapon-Sounds-Reforged.dll"
if (-not (Test-Path $dllSrc)) { throw "DLL not found: $dllSrc" }

New-Item -ItemType Directory -Force -Path $ModDir | Out-Null
Copy-Item -Force $dllSrc (Join-Path $ModDir "Fox-Weapon-Sounds-Reforged.dll")

Write-Host "Copying bundles.json..."
Copy-Item -Force -LiteralPath (Join-Path $Upstream "bundles.json") (Join-Path $ModDir "bundles.json")

Write-Host "Syncing bundles/ (this may take a minute)..."
$bundlesDst = Join-Path $ModDir "bundles"
if (Test-Path -LiteralPath $bundlesDst) {
  Remove-Item -LiteralPath $bundlesDst -Recurse -Force
}
Copy-Item -LiteralPath (Join-Path $Upstream "bundles") -Destination $bundlesDst -Recurse -Force

Write-Host "Deployed to $ModDir"
Write-Host "Restart SPT Server, then test in Hideout. Use skip-list weapons (TT/SKS/VAL/VSS) as canaries for breakage."
