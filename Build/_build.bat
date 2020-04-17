@echo off

if [%1]==[] goto usage

set platform=%1

echo Running: Powershell.exe -executionpolicy remotesigned -File build.ps1 --target=Build --platform=%platform%
Powershell.exe -executionpolicy remotesigned -File build.ps1 --target=Build --platform=%platform%

echo Cake script completed with exit code: %ERRORLEVEL%
exit /B %ERRORLEVEL%

:usage
@echo Usage: %0 ^<platform^>
exit /B 1