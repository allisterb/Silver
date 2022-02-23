@echo off
@setlocal
pushd
set ERROR_CODE=0
dotnet build src\Silver.CLI\Silver.CLI.csproj %*

:end
popd
@endlocal
exit /B %ERROR_CODE%