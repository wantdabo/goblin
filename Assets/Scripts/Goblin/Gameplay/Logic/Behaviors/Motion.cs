using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 运动行为
    /// </summary>
    public class Motion : Behavior<MotionInfo>
    {
        /// <summary>
        /// 运动类型
        /// </summary>
        public void MarkMotion()
        {
            info.motion = MOTION_DEFINE.MOTION;
        }
        
        /// <summary>
        /// 位置类型
        /// </summary>
        public void MarkPosition()
        {
            info.motion = MOTION_DEFINE.POSITION;
        }
    }
}