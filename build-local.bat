@echo off
setlocal

:: Pfade
set SOLUTION=BaseChord.sln
set NUGET_OUTPUT=.\nupkgs
set LOCAL_REPO=..\local-nuget-repo

:: === Version eingeben ===
set /p VERSION=📝 Gib die Package-Version ein (z.B. 1.0.0): 

if "%VERSION%"=="" (
    echo ❌ Keine Version angegeben. Abbruch.
    exit /b 1
)

echo -----------------------------------
echo 🔄 Lösche NuGet Cache...
dotnet nuget locals all --clear

echo -----------------------------------
echo 🔨 Baue Solution: %SOLUTION%
dotnet build %SOLUTION% --configuration Release

echo -----------------------------------
echo 📦 Erstelle NuGet Packages...
dotnet pack %SOLUTION% --configuration Release --output %NUGET_OUTPUT% /p:PackageVersion=%VERSION%

if not exist %LOCAL_REPO% (
    echo 📁 Erstelle lokalen NuGet-Ordner: %LOCAL_REPO%
    mkdir %LOCAL_REPO%
)

echo -----------------------------------
echo 📥 Kopiere NuGet Pakete ins lokale Repository...
for %%f in (%NUGET_OUTPUT%\*.nupkg) do (
    copy /Y "%%f" %LOCAL_REPO% > nul
    echo ✓ Kopiert: %%~nxf
)

echo -----------------------------------
echo ✅ Vorgang abgeschlossen.
endlocal
pause