using System.Collections.Generic;

namespace Kowtow
{
    /// <summary>
    /// 物理层
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// 默认
        /// </summary>
        public const int Default = 1;
        /// <summary>
        /// 地板
        /// </summary>
        public const int Ground = 2;
        /// <summary>
        /// 玩家
        /// </summary>
        public const int Player = 4;
        
        /// <summary>
        /// 物理层检测掩码
        /// </summary>
        private static Dictionary<int, int> maskdict = new()
        {
            { Default, Default | Ground | Player },
            { Ground, Default | Player },
            { Player, Default | Ground },
        };
        
        /// <summary>
        /// 查询层关系
        /// </summary>
        /// <param name="self">自身层</param>
        /// <param name="target">目标层</param>
        /// <returns>YES/NO</returns>
        public static bool Query(int self, int target)
        {
            if (maskdict.TryGetValue(self, out var mask))
            {
                return 0 != (mask & target);
            }

            return false;
        }
    }
}
