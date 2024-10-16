﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Goblin.Core;
using Luban;
using TrueSync;
using UnityEngine;

namespace Goblin.Common
{
    /// <summary>
    /// 游戏配置
    /// </summary>
    public class Config: Comp
    {
        /// <summary>
        /// 配置表定位器
        /// </summary>
        public Tables location { get; set; }

        /// <summary>
        /// 浮点数转整型的乘法系数（1000 表示 1）
        /// </summary>
        public const int float2Int = 1000;

        /// <summary>
        /// 整型转浮点的乘法系数（1000 表示 1）
        /// </summary>
        public const float int2Float = 0.001f;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            location = new Tables((cfgName) => new ByteBuf(engine.gameres.location.LoadConfigSync(cfgName)));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}