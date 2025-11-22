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
        /// <summary>
        /// 是否使用延迟中断状态
        /// </summary>
        public bool usedelaybreak { get; set; }
        /// <summary>
        /// 延迟中断时间
        /// </summary>
        public FP delaybreak { get; set; }
        
        protected override void OnReady()
        {
            current = STATE_DEFINE.NONE;
            last = STATE_DEFINE.NONE;
            usedelaybreak = false;
            delaybreak = FP.Zero;
        }

        protected override void OnReset()
        {
            current = STATE_DEFINE.NONE;
            last = STATE_DEFINE.NONE;
            usedelaybreak = false;
            delaybreak = FP.Zero;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<StateMachineInfo>();
            clone.Ready(actor);
            clone.current = current;
            clone.last = last;
            clone.usedelaybreak = usedelaybreak;
            clone.delaybreak = delaybreak;
            
            return clone;
        }
    }
}