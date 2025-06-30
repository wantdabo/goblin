using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 状态机信息
    /// </summary>
    public class StateMachineInfo : BehaviorInfo
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public byte current { get; set; }
        /// <summary>
        /// 上一个状态
        /// </summary>
        public byte last { get; set; }
        
        protected override void OnReady()
        {
            current = STATE_DEFINE.NONE;
            last = STATE_DEFINE.NONE;
        }

        protected override void OnReset()
        {
            current = STATE_DEFINE.NONE;
            last = STATE_DEFINE.NONE;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<StateMachineInfo>();
            clone.Ready(actor);
            clone.current = current;
            
            return clone;
        }
    }
}