@echo off
@setlocal
set ERROR_CODE=0

src\Silver.CLI\bin\Debug\net6.0\Silver.CLI.exe compile src\Stratis.SmartContracts\Stratis.SmartContracts.csproj --ssc --rewrite --no-assert-rw

echo(

if not %ERRORLEVEL%==0 (
    echo Error compiling Stratis.SmartContracts project.
    set ERROR_CODE=1
    goto end
) else (echo Compiling Stratis.SmartContracts project succeded.)

:end
@endlocal
exit /b %ERROR_CODE%

