using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// Actor 职业生涯信息
    /// </summary>
    public class CareerInfo : BehaviorInfo
    {
        /// <summary>
        /// 出生管线, Actor 在出生时会触发这些管线
        /// </summary>
        public List<uint> bornpipelines { get; set; }
        /// <summary>
        /// 死亡管线, Actor 在死亡时会触发这些管线
        /// </summary>
        public List<uint> deathpipelines { get; set; }
        
        protected override void OnReady()
        {
            bornpipelines = ObjectCache.Ensure<List<uint>>();
            deathpipelines = ObjectCache.Ensure<List<uint>>();
        }

        protected override void OnReset()
        {
            bornpipelines.Clear();
            ObjectCache.Set(bornpipelines);
            
            deathpipelines.Clear();
            ObjectCache.Set(deathpipelines);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<CareerInfo>();
            clone.Ready(actor);
            clone.bornpipelines.AddRange(bornpipelines);
            clone.deathpipelines.AddRange(deathpipelines);
            
            return clone;
        }
    }
}