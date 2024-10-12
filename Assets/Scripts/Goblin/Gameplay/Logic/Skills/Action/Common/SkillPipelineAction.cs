using Goblin.Core;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action.Common
{
    /// <summary>
    /// 技能管线行为
    /// </summary>
    public abstract class SkillPipelineAction : Comp
    {
        /// <summary>
        /// ID/技能行为 ID
        /// </summary>
        public abstract ushort id { get;}
        /// <summary>
        /// 当前执行帧号
        /// </summary>
        public uint frame { get; private set; }
        /// <summary>
        /// 起始帧号
        /// </summary>
        public uint sframe { get; private set; }
        /// <summary>
        /// 结束帧号
        /// </summary>
        public uint eframe { get; private set; }

        public void Enter()
        {
            OnEnter();
        }

        public void Execute(FP tick)
        {
            OnExecute(tick);
        }

        public void Exit()
        {
            OnExit();
        }

        protected abstract void OnEnter();
        protected abstract void OnExecute(FP tick);
        protected abstract void OnExit();
    }
}
