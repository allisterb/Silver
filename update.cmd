@echo off
@setlocal
pushd
set ERROR_CODE=0
git fetch
git pull --recurse-submodules
build
:end
popd
exit /B %ERROR_CODE%