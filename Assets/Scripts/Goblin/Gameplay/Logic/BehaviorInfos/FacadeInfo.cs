using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 外观信息
    /// </summary>
    public class FacadeInfo : BehaviorInfo
    {
        /// <summary>
        /// 模型 ID
        /// </summary>
        public int model { get; set; }
        
        protected override void OnReady()
        {
            model = 0;
        }

        protected override void OnReset()
        {
            model = 0;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<FacadeInfo>();
            clone.Ready(id);
            clone.model = model;
            
            return clone;
        }
    }
}