@echo off

if [%1]==[] goto usage
if [%2]==[] goto usage

set platform=%1
set branch=%2

echo Running: Powershell.exe -executionpolicy remotesigned -File build.ps1 --target=ItchPublish --platform=%platform% --itch_branch=%branch%
Powershell.exe -executionpolicy remotesigned -File build.ps1 --target=ItchPublish --platform=%platform% --itch_branch=%branch%

echo Cake script completed with exit code: %ERRORLEVEL%
exit /B %ERRORLEVEL%

:usage
@echo Usage: %0 ^<platform^> ^<branch^>
exit /B 1