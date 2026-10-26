@echo off
REM Stop any running instances of the application
taskkill /IM FinalProjectWebDeveloper.exe /F 2>nul

REM Wait a moment for the process to fully exit
timeout /t 2 /nobreak

cd /d "%~dp0"
dotnet ef database update

if %ERRORLEVEL% EQU 0 (
	echo.
	echo ====================================
	echo Migration applied successfully!
	echo ====================================
	pause
) else (
	echo.
	echo ====================================
	echo Migration failed. Check the error above.
	echo ====================================
	pause
)
