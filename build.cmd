@echo off
@setlocal
pushd
set ERROR_CODE=0
echo Building Silver CLI project...
dotnet build src\Silver.CLI\Silver.CLI.csproj %*
echo Building Silver notenook projects...
dotnet build src\Silver.Notebooks\Silver.Notebooks.csproj %*
dotnet build src\Silver.Notebooks.DotNetInteractive\Silver.Notebooks.DotNetInteractive.csproj %*
scripts\build-sc-base.cmd
:end
popd
@endlocal
exit /B %ERROR_CODE%