@echo off

echo Preparing Console

dotnet msbuild Apps.sln /target:Database\Console /verbosity:quiet /nologo

dotnet Database\Console\bin\Debug\netcoreapp2.0\Console.dll -h

set /p args="Enter arguments: "

echo.

cd Database\Console\
dotnet bin\Debug\netcoreapp2.0\Console.dll %args%

echo.
pause


