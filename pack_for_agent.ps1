param(
    [switch]$DeployToNpp
)

$ErrorActionPreference = "Stop"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host "  NppOmniView Automated Build & Distribution Packager" -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

$root = $PSScriptRoot
if (-not $root) { $root = (Get-Location).Path }

$patcherProj = Join-Path $root "src\ExportPatcher\ExportPatcher.csproj"
$pluginProj  = Join-Path $root "src\NppOmniView\NppOmniView.csproj"
$pluginDll   = Join-Path $root "src\NppOmniView\bin\Release\net48\NppOmniView.dll"
$patcherExe  = Join-Path $root "src\ExportPatcher\bin\Release\net48\ExportPatcher.exe"
$distDir     = Join-Path $root "dist\NppOmniView"
$nppPluginDir = "D:\Software\Notepad++\plugins\NppOmniView"

Write-Host "`n1. Building ExportPatcher..." -ForegroundColor Yellow
dotnet build $patcherProj -c Release -v minimal
if ($LASTEXITCODE -ne 0) { throw "Failed to build ExportPatcher" }

Write-Host "`n2. Building NppOmniView (Clean Build)..." -ForegroundColor Yellow
dotnet clean $pluginProj -c Release -v quiet
dotnet build $pluginProj -c Release --no-incremental -v minimal
if ($LASTEXITCODE -ne 0) { throw "Failed to build NppOmniView" }

Write-Host "`n3. Running ExportPatcher on NppOmniView.dll..." -ForegroundColor Yellow
& $patcherExe $pluginDll
if ($LASTEXITCODE -ne 0) { throw "ExportPatcher failed to patch NppOmniView.dll" }
Write-Host "   -> PE exports successfully injected into $pluginDll" -ForegroundColor Green

Write-Host "`n4. Running Unit Tests..." -ForegroundColor Yellow
$testScript = Join-Path $root "tests\run_tests.ps1"
if (Test-Path $testScript) {
    & powershell -ExecutionPolicy Bypass -File $testScript
    if ($LASTEXITCODE -ne 0) { throw "Unit tests failed!" }
}

Write-Host "`n5. Packaging to $distDir..." -ForegroundColor Yellow
if (-not (Test-Path $distDir)) {
    New-Item -ItemType Directory -Path $distDir -Force | Out-Null
}

$net48Dir = Join-Path $root "src\NppOmniView\bin\Release\net48"
$binFiles = Get-ChildItem -Path $net48Dir -Filter "*.dll" | Where-Object { $_.Name -notmatch "WebView2\.Wpf\.dll$" }
foreach ($f in $binFiles) {
    Copy-Item $f.FullName -Destination $distDir -Force
}
$redundantWpfDll = Join-Path $distDir "Microsoft.Web.WebView2.Wpf.dll"
if (Test-Path $redundantWpfDll) {
    Remove-Item $redundantWpfDll -Force -ErrorAction SilentlyContinue
}

# Copy native WebView2Loader.dll from runtimes
$nativeLoader = Join-Path $net48Dir "runtimes\win-x64\native\WebView2Loader.dll"
if (Test-Path $nativeLoader) {
    Copy-Item $nativeLoader -Destination $distDir -Force
}

# Copy assets
$assetsDir = Join-Path $root "assets"
if (Test-Path $assetsDir) {
    $assets = Get-ChildItem -Path $assetsDir
    foreach ($a in $assets) {
        Copy-Item $a.FullName -Destination $distDir -Force
    }
}

# Copy sample suite and test assets
$sampleDir = Join-Path $root "tests\samples"
if (Test-Path $sampleDir) {
    $targetSamples = Join-Path $distDir "samples"
    if (-not (Test-Path $targetSamples)) { New-Item -ItemType Directory -Path $targetSamples -Force | Out-Null }
    Copy-Item (Join-Path $sampleDir "*") -Destination $targetSamples -Recurse -Force
}

Write-Host "`n==========================================================" -ForegroundColor Green
Write-Host "  NppOmniView Build & Packaging Complete!" -ForegroundColor Green
Write-Host "  Distribution Folder: $distDir" -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Green

if ($DeployToNpp -or (Test-Path $nppPluginDir)) {
    Write-Host "`nChecking Notepad++ Target Directory: $nppPluginDir" -ForegroundColor Cyan
    $nppProc = Get-Process -Name "notepad++" -ErrorAction SilentlyContinue
    if ($nppProc) {
        Write-Host "  [NOTICE] Notepad++ is currently running (PID: $($nppProc.Id))." -ForegroundColor Yellow
        Write-Host "  The plugin DLL is locked in memory. Please close Notepad++ to overwrite files," -ForegroundColor Yellow
        Write-Host "  or run: .\pack_for_agent.ps1 -DeployToNpp after exiting Notepad++." -ForegroundColor Yellow
    } else {
        Write-Host "  Copying files to $nppPluginDir..." -ForegroundColor Green
        if (-not (Test-Path $nppPluginDir)) {
            New-Item -ItemType Directory -Path $nppPluginDir -Force | Out-Null
        }
        Copy-Item (Join-Path $distDir "*") -Destination $nppPluginDir -Recurse -Force
        Write-Host "  [SUCCESS] Successfully deployed latest NppOmniView to Notepad++!" -ForegroundColor Green
    }
}
