set LUBAN_DLL=../Tools/Luban/Luban.dll
set GEN_CONFIG_CS_DIR=../Cfg/CS
set GEN_CONFIG_JSON_DIR=../Cfg/Json
set CONF_ROOT=../

dotnet --roll-forward LatestMajor %LUBAN_DLL% ^
    -t all ^
    -c cs-dotnet-json ^
    -d json ^
    --conf %CONF_ROOT%/luban.conf ^
    -x outputCodeDir=%GEN_CONFIG_CS_DIR% ^
    -x outputDataDir=%GEN_CONFIG_JSON_DIR%

pause
