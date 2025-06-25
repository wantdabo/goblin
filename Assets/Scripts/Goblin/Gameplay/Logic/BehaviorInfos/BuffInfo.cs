using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class BuffInfo : BehaviorInfo
    {
        public uint buffid { get; set; }
        public uint layer { get; set; }
        public FP lifetime { get; set; }
        public ulong owner { get; set; }
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