@echo off
@setlocal
set ERROR_CODE=0

src\Silver.CLI\bin\Debug\net6.0\Silver.CLI.exe compile src\Stratis.SmartContracts\Stratis.SmartContracts.csproj --ssc --rewrite --no-assert-rw

if not %ERRORLEVEL%==0 (
    echo(
    echo Error compiling Stratis.SmartContracts project.
    set ERROR_CODE=1
    goto end
)

copy src\Stratis.SmartContracts\bin\Debug\net461\ssc\Stratis.SmartContracts.dll lib\net461\Stratis.SmartContracts.NET4.dll
copy src\Stratis.SmartContracts\bin\Debug\net461\ssc\Stratis.SmartContracts.pdb lib\net461\Stratis.SmartContracts.NET4.pdb

copy src\Stratis.SmartContracts\bin\Debug\net461\ssc\Stratis.SmartContracts.dll src\Silver.CLI\bin\Debug\net6.0\Stratis.SmartContracts.NET4.dll
copy src\Stratis.SmartContracts\bin\Debug\net461\ssc\Stratis.SmartContracts.pdb src\Silver.CLI\bin\Debug\net6.0\Stratis.SmartContracts.NET4.pdb

copy src\Stratis.SmartContracts\bin\Debug\net461\ssc\Stratis.SmartContracts.dll src\Silver.CLI\bin\Debug\net6.0\publish\Stratis.SmartContracts.NET4.dll
copy src\Stratis.SmartContracts\bin\Debug\net461\ssc\Stratis.SmartContracts.pdb src\Silver.CLI\bin\Debug\net6.0\publish\Stratis.SmartContracts.NET4.pdb

echo(
echo Building Stratis.SmartContracts base project succeded.

:end
@endlocal
exit /b %ERROR_CODE%

