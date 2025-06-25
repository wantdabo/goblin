using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// Buff 信息
    /// </summary>
    public class BuffInfo : BehaviorInfo
    {
        /// <summary>
        /// BuffID
        /// </summary>
        public uint buffid { get; set; }
        /// <summary>
        /// Buff 层数
        /// </summary>
        public uint layer { get; set; }
        /// <summary>
        /// Buff 生命周期
        /// </summary>
        public FP lifetime { get; set; }
        /// <summary>
        /// Buff 拥有者
        /// </summary>
        public ulong owner { get; set; }
        /// <summary>
        /// Buff 管线
        /// </summary>
        public ulong flow { get; set; }
        
        protected override void OnReady()
        {
            buffid = 0;
            layer = 0;
            lifetime = 0;
            owner = 0;
            flow = 0;
        }

        protected override void OnReset()
        {
            buffid = 0;
            layer = 0;
            lifetime = 0;
            owner = 0;
            flow = 0;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<BuffInfo>();
            clone.Ready(actor);
            clone.buffid = buffid;
            clone.layer = layer;
            clone.lifetime = lifetime;
            clone.owner = owner;
            clone.flow = flow;

            return clone;
        }
    }
}