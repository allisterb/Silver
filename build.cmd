@echo off
@setlocal
pushd
set ERROR_CODE=0
dotnet build src\Silver.CLI\Silver.CLI.csproj %*
scripts\build-sc-base.cmd
:end
popd
@endlocal
exit /B %ERROR_CODE%