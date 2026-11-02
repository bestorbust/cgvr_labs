@echo off
title AutoVerse WebGL Local Server

if exist "%~dp0Builds\WEB-GL\serve_webgl.py" (
    cd /d "%~dp0Builds\WEB-GL"
    goto RUN
)

if exist "%~dp0Builds\WebGL\serve_webgl.py" (
    cd /d "%~dp0Builds\WebGL"
    goto RUN
)

echo [ERROR] Could not find WebGL build folder!
pause
exit /b 1

:RUN
echo ========================================================
echo  Starting AutoVerse WebGL Server on http://localhost:8080
echo  Serving from: %CD%
echo ========================================================
python serve_webgl.py
pause
