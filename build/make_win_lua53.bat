mkdir build32_53 & pushd build32_53
cmake -G "Visual Studio 16 2019" -A "Win32" ..
popd
cmake --build build32_53 --config Release
md plugin_lua53\Plugins\x86
copy /Y build32_53\Release\xlua.dll plugin_lua53\Plugins\x86\xlua.dll

mkdir build64_53 & pushd build64_53
cmake -G "Visual Studio 16 2019" -A "x64" ..
popd
cmake --build build64_53 --config Release
md plugin_lua53\Plugins\x86_64
copy /Y build64_53\Release\xlua.dll plugin_lua53\Plugins\x86_64\xlua.dll

pause