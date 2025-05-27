using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.BehaviorInfos.Flows
{
    /// <summary>
    /// 碰撞信息
    /// </summary>
    public class CollisionInfo : BehaviorInfo
    {
        /// <summary>
        /// 碰撞的 ActorID 列表
        /// </summary>
        public Queue<ulong> collisions { get; set; }
        
        protected override void OnReady()
        {
            collisions = ObjectCache.Ensure<Queue<ulong>>();
        }

        protected override void OnReset()
        {
            collisions.Clear();
            ObjectCache.Set(collisions);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<CollisionInfo>();
            clone.Ready(actor);
            foreach (var id in collisions)
            {
                clone.collisions.Enqueue(id);
            }
            
            return clone;
        }
    }
}