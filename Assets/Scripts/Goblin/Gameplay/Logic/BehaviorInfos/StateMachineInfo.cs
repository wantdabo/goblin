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
        /// 持续帧数
        /// </summary>
        public uint frames { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public FP elapsed { get; set; }
        
        protected override void OnReady()
        {
            current = STATE_DEFINE.IDLE;
            frames = 0;
            elapsed = FP.Zero;
        }

        protected override void OnReset()
        {
            current = STATE_DEFINE.IDLE;
            frames = 0;
            elapsed = FP.Zero;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<StateMachineInfo>();
            clone.Ready(id);
            clone.current = current;
            clone.frames = frames;
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + id.GetHashCode();
            hash = hash * 31 + current.GetHashCode();
            hash = hash * 31 + frames.GetHashCode();
            hash = hash * 31 + elapsed.GetHashCode();
            
            return hash;
        }
    }
}