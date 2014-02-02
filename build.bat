@echo off

set msbuild=C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe

set projectfile=build\build.proj

 :start
call %BuildCommonDir%\prebuild.bat

set target=
if NOT "%1" == "" set target=/t:%1
 %msbuild% %_loglevel% %target% %projectfile%
shift
if "%1" == "" goto nomoreparams
goto start

 :nomoreparams
if ERRORLEVEL 1 goto error
echo completed
goto end

 :error
echo Failed
goto end

 :end