@echo off
rem delete *.nupkg
if exist "bin\app\" (
    rmdir /s /q "bin\app
)

rem "publish..."
dotnet publish -c Release -o bin\app
IF %ERRORLEVEL% GTR 0 (
    PAUSE
    GOTO END
)

:END
exit;