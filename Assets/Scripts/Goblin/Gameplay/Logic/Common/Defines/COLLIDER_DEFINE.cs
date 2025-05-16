using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// 碰撞盒定义
    /// </summary>
    public class COLLIDER_DEFINE
    {
        /// <summary>
        /// 立方体
        /// </summary>
        public const byte BOX = 1;
        /// <summary>
        /// 球体
        /// </summary>
        public const byte SPHERE = 2;
        
        
        
        /// <summary>
        /// 默认
        /// </summary>
        public const int LAYER_DEFAULT = 1;
        /// <summary>
        /// 地板
        /// </summary>
        public const int LAYER_GROUND = 2;
        /// <summary>
        /// 玩家
        /// </summary>
        public const int LAYER_PLAYER = 4;
        
        /// <summary>
        /// 物理层检测掩码
        /// </summary>
        private static Dictionary<int, int> maskdict = new()
        {
            { LAYER_DEFAULT, LAYER_DEFAULT | LAYER_GROUND | LAYER_PLAYER },
            { LAYER_GROUND, LAYER_DEFAULT | LAYER_PLAYER },
            { LAYER_PLAYER, LAYER_DEFAULT | LAYER_GROUND },
        };
        
        /// <summary>
        /// 查询层关系
        /// </summary>
        /// <param name="self">自身层</param>
        /// <param name="target">目标层</param>
        /// <returns>YES/NO</returns>
        public static bool QUERY(int self, int target)
        {
            if (maskdict.TryGetValue(self, out var mask))
            {
                return 0 != (mask & target);
            }

            return false;
        }
    }
}