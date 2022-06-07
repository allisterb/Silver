@echo off
@setlocal

echo Building Silver notenook projects...
dotnet build src\Silver.Notebooks\Silver.Notebooks.csproj %*
dotnet build src\Silver.Notebooks.DotNetInteractive\Silver.Notebooks.DotNetInteractive.csproj %*

:end
@endlocal