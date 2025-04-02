using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 运动信息
    /// </summary>
    public class MovementInfo : BehaviorInfo
    {
        /// <summary>
        /// 当前帧在运动
        /// </summary>
        public bool moving { get; set; }
        
        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            moving = false;
        }
    }
}