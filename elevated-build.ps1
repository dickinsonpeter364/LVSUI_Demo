#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Build the LVSUi C++ COM projects and register the resulting DLLs.
    Must be run as Administrator (required for regsvr32 and RegisterOutput in vcxproj).

.USAGE
    Right-click PowerShell → "Run as Administrator", then:
        & C:\LVSUi\elevated-build.ps1
    Or pass -Config and/or -SkipBuild:
        & C:\LVSUi\elevated-build.ps1 -Config Debug
        & C:\LVSUi\elevated-build.ps1 -SkipBuild
#>
param(
    [ValidateSet('Release','Debug','Both')]
    [string]$Config  = 'Both',

    [switch]$SkipBuild    # Just re-register already-built DLLs
)

$ErrorActionPreference = 'Continue'
$Root = 'C:\LVSUi'
$Sln  = "$Root\LvsUI-Full.sln"

# ── 1. Locate VS MSBuild via vswhere ─────────────────────────────────────────
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) {
    Write-Error "vswhere.exe not found at $vswhere. Is Visual Studio installed?"
    exit 1
}

$vsInstall = & $vswhere -latest -products * `
    -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 `
    -property installationPath 2>$null

if (-not $vsInstall) {
    Write-Error "No VS install with C++ tools found. Install the 'Desktop development with C++' workload."
    exit 1
}

# Use the 64-bit MSBuild for speed
$msbuild = "$vsInstall\MSBuild\Current\Bin\amd64\MSBuild.exe"
if (-not (Test-Path $msbuild)) {
    $msbuild = "$vsInstall\MSBuild\Current\Bin\MSBuild.exe"   # fallback to 32-bit
}
Write-Host "=== Using MSBuild: $msbuild ===" -ForegroundColor Cyan
Write-Host "=== VS Install:    $vsInstall ===" -ForegroundColor Cyan

# ── 2. Set vcpkg root (required for OpenCV / poppler / tesseract headers) ────
$env:VCPKG_ROOT = 'C:\VCPKG'
Write-Host "=== VCPKG_ROOT:    $env:VCPKG_ROOT ===" -ForegroundColor Cyan

# ── 3. Helper: run one MSBuild pass ──────────────────────────────────────────
function Invoke-MSBuild([string]$cfg) {
    $log = "$Root\build-$($cfg.ToLower()).log"
    Write-Host ""
    Write-Host "=== Building $cfg|x64 ===" -ForegroundColor Yellow
    & $msbuild $Sln -m `
        "-p:Configuration=$cfg" '-p:Platform=x64' `
        '-v:minimal' '--nologo' `
        *>&1 | Tee-Object -FilePath $log
    return $LASTEXITCODE
}

# ── 4. Clean orphan CLSID registrations (known stale keys) ───────────────────
Write-Host ""
Write-Host "=== Cleaning orphan CLSID registrations ===" -ForegroundColor Cyan
$orphans = @('{fd084d21-d1f5-4400-abe4-35035567ede3}')
foreach ($clsid in $orphans) {
    foreach ($root in @('HKLM:\SOFTWARE\Classes\CLSID','HKLM:\SOFTWARE\Classes\Wow6432Node\CLSID')) {
        $path = Join-Path $root $clsid
        if (Test-Path $path) {
            Write-Host "  Removing $path"
            Remove-Item $path -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

# ── 5. Build ──────────────────────────────────────────────────────────────────
$exitCodes = @{}

if (-not $SkipBuild) {
    $configs = switch ($Config) {
        'Both'    { @('Debug','Release') }
        default   { @($Config) }
    }

    foreach ($cfg in $configs) {
        $exitCodes[$cfg] = Invoke-MSBuild $cfg
        if ($exitCodes[$cfg] -ne 0) {
            Write-Warning "Build FAILED for $cfg|x64 (exit $($exitCodes[$cfg])). See $Root\build-$($cfg.ToLower()).log"
        } else {
            Write-Host "  Build OK for $cfg|x64" -ForegroundColor Green
        }
    }
} else {
    Write-Host "=== Skipping build (-SkipBuild) ===" -ForegroundColor Yellow
}

# ── 6. Force-register COM DLLs ────────────────────────────────────────────────
# All C# and C++ outputs are redirected to C:\LVSRun\<Config>\ via
# Directory.Build.props (C#) and the <OutDir> entries in each vcxproj (C++).
$RunRoot = 'C:\LVSRun'
$comDlls = [ordered]@{
    "$RunRoot\Release\OpenCVComMatcher.dll"   = 'OpenCVComMatcher (Release)'
    "$RunRoot\Release\ImageProcessorCom.dll"  = 'ImageProcessorCom (Release)'
    "$RunRoot\Debug\OpenCVComMatcher.dll"     = 'OpenCVComMatcher (Debug)'
    "$RunRoot\Debug\ImageProcessorCom.dll"    = 'ImageProcessorCom (Debug)'
}

Write-Host ""
Write-Host "=== Registering COM DLLs ===" -ForegroundColor Cyan
foreach ($dll in $comDlls.Keys) {
    $label = $comDlls[$dll]
    if (Test-Path $dll) {
        Write-Host "  regsvr32 /s $dll  [$label]"
        & regsvr32.exe /s $dll
        if ($LASTEXITCODE -eq 0) {
            Write-Host "    OK" -ForegroundColor Green
        } else {
            Write-Warning "    regsvr32 failed (exit $LASTEXITCODE) for $dll"
        }
    } else {
        Write-Warning "  MISSING (not built?): $dll"
    }
}

# ── 7. Summary ────────────────────────────────────────────────────────────────
$summary = @()
foreach ($cfg in $exitCodes.Keys) {
    $summary += "$($cfg.ToUpper())_EXIT=$($exitCodes[$cfg])"
}
$summary += "REGISTERED=$(($comDlls.Keys | Where-Object { Test-Path $_ }).Count)_of_$($comDlls.Count)"
$summary += "TIMESTAMP=$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')"
$summary | Out-File "$Root\build-summary.txt" -Encoding ascii

Write-Host ""
Write-Host "=== Done. Summary written to $Root\build-summary.txt ===" -ForegroundColor Cyan
$summary | ForEach-Object { Write-Host "  $_" }
