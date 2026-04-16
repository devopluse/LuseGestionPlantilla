@echo off
echo Iniciando LuseGestion API...
echo.
echo La API estará disponible en: http://localhost:5039/
echo Swagger UI estará disponible en: http://localhost:5039/
echo.
echo Presiona Ctrl+C para detener la aplicación
echo ========================================
echo.

cd /d "%~dp0"

dotnet run

pause
