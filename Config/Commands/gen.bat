set GEN_CLIENT=../Tools/Luban.ClientServer/Luban.ClientServer.dll
set GEN_CONFIG_CS_DIR=../Cfg/CS
set GEN_CONFIG_Bytes_DIR=../Cfg/Bytes

dotnet %GEN_CLIENT% -j cfg --^
 -d ../Defines/__root__.xml ^
 --input_data_dir ../Datas ^
 --output_code_dir %GEN_CONFIG_CS_DIR% ^
 --output_data_dir %GEN_CONFIG_Bytes_DIR% ^
 --gen_types code_cs_bin,data_bin ^
 -s all
 pause
