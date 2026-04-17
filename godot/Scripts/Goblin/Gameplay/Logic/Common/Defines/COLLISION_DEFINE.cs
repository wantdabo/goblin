using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// 碰撞定义
    /// </summary>
    public class COLLISION_DEFINE
    {
        /// <summary>
        /// 立方体
        /// </summary>
        public const byte COLLIDER_BOX = 1;
        /// <summary>
        /// 球体
        /// </summary>
        public const byte COLLIDER_SPHERE = 2;

        /// <summary>
        /// 碰撞类型 - 攻击盒
        /// </summary>
        public const byte COLLISION_TYPE_HURT = 1;
        /// <summary>
        /// 碰撞类型 - 嗅探器
        /// </summary>
        public const byte COLLISION_TYPE_SENSOR = 2;

        /// <summary>
        /// 碰撞射线
        /// </summary>
        public const byte COLLISION_RAY = 1;
        /// <summary>
        /// 碰撞线段
        /// </summary>
        public const byte COLLISION_LINE = 2;
        /// <summary>
        /// 碰撞立方体
        /// </summary>
        public const byte COLLISION_BOX = 3;
        /// <summary>
        /// 碰撞球体
        /// </summary>
        public const byte COLLISION_SPHERE = 4;
        
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