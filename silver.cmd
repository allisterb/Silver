@echo off
@setlocal
pushd
set ERROR_CODE=0
src\Silver.CLI\bin\Debug\net6.0\Silver.CLI.exe %*

:end
popd
exit /B %ERROR_CODE%