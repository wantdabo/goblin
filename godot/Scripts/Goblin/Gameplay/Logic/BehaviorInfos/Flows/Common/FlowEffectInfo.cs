using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common
{
    /// <summary>
    /// 管线特效信息
    /// </summary>
    public class FlowEffectInfo : BehaviorInfo
    {
        /// <summary>
        /// 管线特效 ID 列表
        /// </summary>
        public List<uint> effects { get; set; }
        
        protected override void OnReady()
        {
            effects = ObjectCache.Ensure<List<uint>>();
        }

        protected override void OnReset()
        {
            effects.Clear();
            ObjectCache.Set(effects);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<FlowEffectInfo>();
            clone.Ready(actor);
            clone.effects.AddRange(effects);
            
            return clone;
        }
    }
}