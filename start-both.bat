@echo off
echo Starting CareFusion API and Web Application...

echo.
echo Starting WebAPI on https://localhost:5001...
start "CareFusion WebAPI" cmd /k "cd /d CareFusion.WebApi && dotnet run"

echo.
echo Waiting 10 seconds for API to start...
timeout /t 10 /nobreak >nul

echo.
echo Starting Web Application on https://localhost:5000...
start "CareFusion Web" cmd /k "cd /d CareFusion.Web && dotnet run"

echo.
echo Both applications are starting...
echo WebAPI: https://localhost:5001
echo Web App: https://localhost:5000
echo.
echo Press any key to exit this window (applications will continue running)
pause >nul