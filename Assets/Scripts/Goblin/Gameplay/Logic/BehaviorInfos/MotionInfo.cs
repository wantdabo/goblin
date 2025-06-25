using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 运动信息
    /// </summary>
    public class MotionInfo : BehaviorInfo
    {
        /// <summary>
        /// 运动类型
        /// </summary>
        public byte motion { get; set; }
        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            motion = 0;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<MotionInfo>();
            clone.Ready(actor);
            clone.motion = motion;
            
            return clone;
        }
    }
}