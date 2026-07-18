#!/bin/bash

# 复制配置 JSON 文件
cp -Rf "$PWD/../Cfg/Json/"* "$PWD/../../godot/GameRes/Raw/Configs/"
# 复制配置源码文件
cp -Rf "$PWD/../Cfg/CS/"* "$PWD/../../godot/Scripts/Goblin/Common/"

read -p "press enter continue..." dummy
