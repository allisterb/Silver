@echo off
@setlocal
pushd
set ERROR_CODE=0
dotnet clean src\Silver.CLI\Silver.CLI.csproj %*

:end
popd
exit /B %ERROR_CODE%