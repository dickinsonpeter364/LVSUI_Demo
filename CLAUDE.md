# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

LVSUI is a WPF/MVVM Windows desktop application (.NET 10.0-windows) for automated label inspection on a manufacturing line. It integrates live camera feeds, OpenCV-based image matching via C++ COM DLLs, a PLC over TCP/IP, BDaq hardware I/O, and an Oracle database.

## Build

**C# only (normal development):**
```
dotnet build LvsUI.slnx -p:Platform=x64
```

**Full build including C++ COM DLLs (must run as Administrator):**
```powershell
& C:\LVSUI\elevated-build.ps1              # Debug + Release
& C:\LVSUI\elevated-build.ps1 -Config Debug
& C:\LVSUI\elevated-build.ps1 -SkipBuild   # re-register already-built DLLs only
```
The script finds VS via vswhere, sets `VCPKG_ROOT=C:\VCPKG`, builds `LvsUI-Full.sln` (x64), and registers COM DLLs with regsvr32. Logs go to `build-debug.log` / `build-release.log`.

Always target **x64**. The solution has no meaningful Any CPU configuration.

## Tests

The only test project is `Libraries/ImageProc/ComMatcherTest/` — it tests COM interop with the image-matching DLLs and requires the C++ build to have run first. There is no unit test suite.

## Key Configuration (`appsettings.json`)

| Key | Effect |
|-----|--------|
| `MxClient` | `"dummy"` = software-only mode; `"default"` = real camera hardware |
| `BypassSecurity` | `true` = skip AD/Oracle group authentication |
| `CaptureOnly` | `true` = run without full inspection pipeline |
| `LafCaptureTest` | `true` = clip PDF to content rect and display immediately |
| `DebugMode` | Enables extra logging |
| `StationID` | Which inspection station this instance controls |

On the dev machine (no hardware), keep `MxClient=dummy` and `BypassSecurity=true`. On the test machine (BDaq, Oracle, PLC), flip these to real values.

## Architecture

**MVVM layers:**
- `Views/` — XAML views
- `ViewModels/` — CommunityToolkit.Mvvm ViewModels
- `Services/` — business logic (CameraService, LabelMatcherService, AlarmService, etc.)
- `Core/` — ViewModelBase, RelayCommand, DI setup, app configuration

DI is wired in `App.xaml.cs` using `Microsoft.Extensions.DependencyInjection`. Logging uses Serilog → `Logs\log.txt` (daily rolling).

**Libraries under `Libraries/`:**

| Library | Role |
|---------|------|
| `ApplicationData` | Shared data models and application state |
| `DataAccessOracle` | Oracle DB access (Oracle.ManagedDataAccess.Core) |
| `DataManager` | Higher-level data operations over DataAccessOracle |
| `Inspection` | Core inspection logic; active implementation is `DummyInspectionStub` |
| `CameraManager` | Camera acquisition and lifecycle |
| `mxClient` | MxPEG camera library integration |
| `Interfacing` | PLC and hardware interfaces |
| `IOClasses` | BDaq I/O device classes; device layout in `DeviceIO.xml` |
| `Security` | AD/LDAP auth + Oracle group check; audit trail |
| `Messaging` | Inter-component event bus |
| `ImageProc` (C++) | OpenCV wrapper compiled to COM DLLs |

Libraries not yet wired into the solution (source files exist in `Libraries/` but lack project references): Document, InspectionManagement, Peripherals, Signatures, UserControls, USBLib, MELSEC, mxServer, SharedClasses, StaticClasses.

**C++/C# COM interop:**
`ImageProc/` contains C++ projects that build `OpenCVComMatcher.dll` and `ImageProcessorCom.dll` (registered in-proc COM servers). C# calls them through generated interop wrappers. The full solution `LvsUI-Full.sln` includes these; `LvsUI.slnx` is C#-only.

**Alarm pipeline:** BDaq hardware I/O → `IOClasses` events → ViewModel → `InspectionView` alarm panel.

## Solutions

| File | Use when |
|------|----------|
| `LvsUI.slnx` | C# work only (faster, no C++ toolchain needed) |
| `LvsUI-Full.sln` | Any change touching `Libraries/ImageProc/` or COM DLLs |

## Submodule: Libraries/ImageProc

`Libraries/ImageProc` is a git submodule pointing at `dickinsonpeter364/ImageProc_Demo`. After cloning LVSUI_Demo, initialise it with:

```
git submodule update --init --recursive
```

To update the C++ COM code to the latest ImageProc_Demo commit:

```
cd Libraries/ImageProc
git pull origin master
cd ../..
git add Libraries/ImageProc
git commit -m "chore: bump ImageProc submodule"
```
