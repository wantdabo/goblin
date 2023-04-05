using System.Collections.Generic;
using System.Threading.Tasks;
using Bright.Serialization;
using GoblinFramework.Client.Common;

namespace GoblinFramework.Common
{
    public class GameConfig : CComp
    {
        /// <summary>
        /// 配置表定位器
        /// </summary>
        public Tables Location;

        /// <summary>
        /// 配置表的名字
        /// </summary>
        private List<string> cfgNames = new List<string>()
        {
            "Conf.Gameplay.UpgradeInfos",
        };

        /// <summary>
        /// 预加载所有配置的 bytes
        /// </summary>
        private Dictionary<string, byte[]> cfgBytesDict = new Dictionary<string, byte[]>();

        /// <summary>
        /// 初始化配置表
        /// </summary>
        /// <returns>Task</returns>
        public async Task InitialGameCfg()
        {
            foreach (var cfgName in cfgNames)
            {
                var bytes = await Engine.GameRes.Location.LoadConfigAsync(cfgName);
                cfgBytesDict.Add(cfgName, bytes);
            }
            Location = new Tables((cfgName) => new ByteBuf(cfgBytesDict[cfgName]));
        }
    }
}