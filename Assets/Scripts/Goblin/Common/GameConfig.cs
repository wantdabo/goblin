using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Bright.Serialization;
using Goblin.Core;
using UnityEngine;

namespace Goblin.Common
{
    /// <summary>
    /// 游戏配置
    /// </summary>
    public class GameConfig: Comp
    {
        /// <summary>
        /// 配置表定位器
        /// </summary>
        public Tables location;

        /// <summary>
        /// 浮点数转整型的乘法系数（10000 表示 1）
        /// </summary>
        public int float2Int = 10000;

        /// <summary>
        /// 整型转浮点的乘法系数（10000 表示 1）
        /// </summary>
        public float int2Float = 0.0001f;

        /// <summary>
        /// 配置表的名字
        /// </summary>
        private List<string> cfgNames = new()
        {
            "Conf.ItemInfo",
        };

        /// <summary>
        /// 预加载所有配置的 bytes
        /// </summary>
        private Dictionary<string, byte[]> cfgBytesDict = new();

        /// <summary>
        /// 初始化配置表
        /// </summary>
        /// <returns>Task</returns>
        public async Task Initial()
        {
            foreach(var cfgName in cfgNames)
            {
                var bytes = await engine.gameRes.location.LoadConfigAsync(cfgName);
                cfgBytesDict.Add(cfgName, bytes);
            }
            location = new Tables((cfgName) => new ByteBuf(cfgBytesDict[cfgName]));
        }
    }
}