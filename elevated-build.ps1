$ErrorActionPreference = 'Continue'
$env:VCPKG_ROOT = 'C:\VCPKG'
$msbuild = 'C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe'
$sln     = 'C:\LVSUI\LvsUI-Full.sln'

Write-Host "=== Cleaning orphan CLSID registrations ==="
$orphans = @('{fd084d21-d1f5-4400-abe4-35035567ede3}')
foreach ($clsid in $orphans) {
    foreach ($root in @('HKLM:\SOFTWARE\Classes\CLSID','HKLM:\SOFTWARE\Classes\Wow6432Node\CLSID')) {
        $path = Join-Path $root $clsid
        if (Test-Path $path) {
            Write-Host "Removing $path"
            Remove-Item $path -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

Write-Host "=== DEBUG|x64 ==="
& $msbuild $sln -m '-p:Configuration=Debug' '-p:Platform=x64' '-v:minimal' *>&1 | Tee-Object -FilePath C:\LVSUI\build-debug.log
$debugExit = $LASTEXITCODE

Write-Host "=== RELEASE|x64 ==="
& $msbuild $sln -m '-p:Configuration=Release' '-p:Platform=x64' '-v:minimal' *>&1 | Tee-Object -FilePath C:\LVSUI\build-release.log
$releaseExit = $LASTEXITCODE

Write-Host "=== Force-registering COM DLLs (Release) ==="
foreach ($dll in @('C:\LVSUI\x64\Release\ImageProcessorCom.dll','C:\LVSUI\x64\Release\OpenCVComMatcher.dll')) {
    if (Test-Path $dll) {
        Write-Host "regsvr32 /s $dll"
        & regsvr32.exe /s $dll
    } else {
        Write-Host "MISSING: $dll"
    }
}

"DEBUG_EXIT=$debugExit"     | Out-File C:\LVSUI\build-summary.txt -Encoding ascii
"RELEASE_EXIT=$releaseExit" | Out-File C:\LVSUI\build-summary.txt -Append -Encoding ascii
