using System.IO;
using Kowtow.Math;
using Luban;

namespace Goblin.Gameplay.Logic.Common
{
    /// <summary>
    /// 配置数据
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 配置表定位器
        /// </summary>
        public static Tables location { get; private set; }
        /// <summary>
        /// FP 转整型的乘法系数（1000 表示 1）
        /// </summary>
        public static int FP2Int { get; private set; } = 1000;
        /// <summary>
        /// 整型转 FP 的乘法系数（1000 表示 1）
        /// </summary>
        public static FP Int2FP { get; private set; } = FP.EN3;

        static Config()
        {
            string path = string.Empty;
#if UNITY_EDITOR
            path = $"{UnityEngine.Application.dataPath}/GameRes/Raw/Configs/";
#endif
            location = new Tables((cfgName) => new ByteBuf(File.ReadAllBytes($"{path}{cfgName}.bytes")));
        }
    }
}