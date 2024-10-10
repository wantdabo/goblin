using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Behaviors.Common
{
    /// <summary>
    /// 动画播放
    /// </summary>
    public abstract class Animation : Behavior
    {
        /// <summary>
        /// 当前播放的动画名
        /// </summary>
        public string[] names { get; protected set; } = new string[StateDef.MAX_LAYER];
        /// <summary>
        /// 播放动画
        /// </summary>
        /// <param name="name">动画名</param>
        /// <param name="layer">层级</param>
        public abstract void Play(string name, byte layer = 0);
    }
}
