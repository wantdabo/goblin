using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Collisions
{
    /// <summary>
    /// 碰撞盒
    /// </summary>
    public class ColliderInfo : BehaviorInfo
    {
        /// <summary>
        /// 碰撞层
        /// </summary>
        public int layer { get; set; }
        /// <summary>
        /// 几何体类型
        /// </summary>
        public byte shape { get; set; }
        /// <summary>
        /// 立方体
        /// </summary>
        public Box box { get; set; }
        /// <summary>
        /// 球体
        /// </summary>
        public Sphere sphere { get; set; }
        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            layer = COLLIDER_DEFINE.LAYER_DEFAULT;
            shape = 0;
            box = default;
            sphere = default;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<ColliderInfo>();
            clone.Ready(id);
            clone.layer = layer;
            clone.shape = shape;
            clone.box = box;
            clone.sphere = sphere;

            return clone;
        }
    }
}