using Goblin.Gameplay.Render.Resolvers.Common;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers.States
{
    /// <summary>
    /// 空间状态
    /// </summary>
    public struct SpatialState : IState
    {
        public StateType type => StateType.Spatial;
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong actor { get; set; }
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        public Vector3 position { get; set; }
        public Vector3 euler { get; set; }
        public Vector3 scale { get; set; }
    }
}