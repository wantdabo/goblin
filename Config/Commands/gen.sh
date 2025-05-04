#!/bin/bash

LUBAN_DLL="../Tools/Luban/Luban.dll"
GEN_CONFIG_CS_DIR="../Cfg/CS"
GEN_CONFIG_Bytes_DIR="../Cfg/Bytes"
CONF_ROOT="../"

dotnet $LUBAN_DLL \
    -t all \
    -c cs-bin \
    -d bin \
    --conf $CONF_ROOT/luban.conf \
    -x outputCodeDir=$GEN_CONFIG_CS_DIR \
    -x outputDataDir=$GEN_CONFIG_Bytes_DIR

read -p "press enter continue..." dummy