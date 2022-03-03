@echo off
@setlocal
set ERROR_CODE=0

dotnet build src\Silver.CodeAnalysis.Cs\Silver.CodeAnalysis.Cs.Package\Silver.CodeAnalysis.Cs.Package.csproj 

:end
@endlocal
exit /b %ERROR_CODE%

