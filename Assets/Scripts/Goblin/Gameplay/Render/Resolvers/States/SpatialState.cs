using Goblin.Gameplay.Render.Resolvers.Common;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 空间数据状态
    /// </summary>
    public class SpatialState : State
    {
        public override StateType type => StateType.Spatial;
        /// <summary>
        /// 位置
        /// </summary>
        public Vector3 position { get; set; }
        /// <summary>
        /// 旋转
        /// </summary>
        public Vector3 euler { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
        public Vector3 scale { get; set; }
        
        protected override void OnReset()
        {
            position = Vector3.zero;
            euler = Vector3.zero;
            scale = Vector3.one;
        }
    }
}