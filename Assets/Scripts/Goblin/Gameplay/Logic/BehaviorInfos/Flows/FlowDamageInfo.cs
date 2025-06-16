using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Flows
{
    /// <summary>
    /// 流程伤害信息
    /// </summary>
    public class FlowDamageInfo : BehaviorInfo
    {
        /// <summary>
        /// 碰撞的 ActorID 列表
        /// </summary>
        public Queue<(ulong actor, (uint pipeline, uint index) identity)> targets { get; set; }
        
        protected override void OnReady()
        {
            targets = ObjectCache.Ensure<Queue<(ulong actor, (uint pipeline, uint index))>>();
        }

        protected override void OnReset()
        {
            targets.Clear();
            ObjectCache.Set(targets);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<FlowDamageInfo>();
            clone.Ready(actor);
            foreach (var id in targets)
            {
                clone.targets.Enqueue(id);
            }
            
            return clone;
        }
    }
}