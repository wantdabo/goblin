#if UNITY_EDITOR
using System.IO;
using Luban;
using UnityEditor;
using UnityEngine;

namespace Goblin.Misc
{
    /// <summary>
    /// 编辑器配置
    /// </summary>
    [InitializeOnLoad]
    public class EditorConfig
    {
        /// <summary>
        /// 配置表定位器
        /// </summary>
        public static Tables location { get; private set; }

        static EditorConfig()
        {
            location = new Tables((cfg) =>
            {
                return new ByteBuf( File.ReadAllBytes($"{Application.dataPath}/GameRes/Raw/Configs/{cfg}.bytes"));
            });
        }
    }
}
#endif