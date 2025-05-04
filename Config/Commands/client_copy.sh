#!/bin/bash

# 复制配置二进制文件
cp -Rf "$PWD/../Cfg/Bytes/"* "$PWD/../../Assets/GameRes/Raw/Configs/"
# 复制配置源码文件
cp -Rf "$PWD/../Cfg/CS/"* "$PWD/../../Assets/Scripts/Goblin/Common"

read -p "press enter continue..." dummy