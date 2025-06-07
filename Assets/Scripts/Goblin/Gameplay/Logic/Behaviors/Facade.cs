using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 外观行为
    /// </summary>
    public class Facade : Behavior<FacadeInfo>
    {
        /// <summary>
        /// 设置模型
        /// </summary>
        /// <param name="model">模型 ID</param>
        public void SetModel(int model)
        {
            info.model = model;
        }

        /// <summary>
        /// 设置动画状态
        /// </summary>
        /// <param name="state">动画状态</param>
        /// <param name="elapsed">时间流逝</param>
        public void SetAnimationState(byte state, FP elapsed)
        {
            info.animstate = state;
            info.animelapsed = elapsed;
        }
        
        /// <summary>
        /// 设置动画状态
        /// </summary>
        /// <param name="state">动画状态</param>
        public void SetAnimationState(byte state)
        {
            info.animstate = state;
        }

        /// <summary>
        /// 设置动画名称
        /// </summary>
        /// <param name="name">动画名称</param>
        /// <param name="elapsed">时间流逝</param>
        public void SetAnimationName(string name, FP elapsed)
        {
            info.animname = name;
            info.animelapsed = elapsed;
        }
    }
}