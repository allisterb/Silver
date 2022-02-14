@echo off
@setlocal
set ERROR_CODE=0
git fetch && git pull --recurse-submodules && build

:end
exit /B %ERROR_CODE%
