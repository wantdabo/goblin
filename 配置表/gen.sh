#!/bin/zsh
GEN_CLIENT=./Tools/Luban.ClientServer/Luban.ClientServer.dll
GEN_CONFIG_CS_DIR=./Assets/Scripts/GoblinFramework/Common
GEN_CONFIG_Bytes_DIR=./Assets/GameRawRes/Configs

dotnet ${GEN_CLIENT} -j cfg --\
 -d Defines/__root__.xml \
 --input_data_dir Datas \
 --output_code_dir ${GEN_CONFIG_CS_DIR} \
 --output_data_dir ${GEN_CONFIG_Bytes_DIR} \
 --gen_types code_cs_bin,data_bin \
 -s all