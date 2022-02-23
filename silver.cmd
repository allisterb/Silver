@echo off
pushd
@setlocal
set ERROR_CODE=0

src\Silver.CLI\bin\Debug\net6.0\Silver.CLI.exe %*

:end
@endlocal
popd
exit /B %ERROR_CODE%