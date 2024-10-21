using Goblin.Core;
using Goblin.Gameplay.Common.SkillDatas.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action.Common
{
    /// <summary>
    /// 技能管线行为
    /// </summary>
    public abstract class SkillAction : Comp
    {
        /// <summary>
        /// ID/技能行为 ID
        /// </summary>
        public abstract ushort id { get; }
        /// <summary>
        /// 技能管线
        /// </summary>
        public SkillPipeline pipeline { get; set; }

        public void Enter(SkillActionData data)
        {
            OnEnter(data);
        }

        public void Execute(SkillActionData data, uint frame, FP tick)
        {
            OnExecute(data, tick);
        }
        
        public void Exit(SkillActionData data)
        {
            OnExit(data);
        }

        protected abstract void OnEnter(SkillActionData data);
        protected abstract void OnExecute(SkillActionData data, FP tick);
        protected abstract void OnExit(SkillActionData data);
    }

    public abstract class SkillAction<T> : SkillAction where T : SkillActionData
    {

        protected override void OnEnter(SkillActionData data)
        {
            OnEnter((T)data);
        }
        
        protected override void OnExecute(SkillActionData data, FP tick)
        {
            OnExecute((T)data, tick);
        }

        protected override void OnExit(SkillActionData data)
        {
            OnExit((T)data);
        }

        protected virtual void OnEnter(T data) { }
        protected abstract void OnExecute(T data, FP tick);
        protected virtual void OnExit(T data) { }
    }
}
