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
        public SkillPipeline sp { get; set; }

        public void Execute(SkillActionData data, uint frame, FP tick)
        {
            OnExecute(data, tick);
        }

        protected abstract void OnExecute(SkillActionData data, FP tick);
    }

    public abstract class SkillAction<T> : SkillAction where T : SkillActionData
    {
        protected override void OnExecute(SkillActionData data, FP tick)
        {
            OnExecute((T)data, tick);
        }

        protected abstract void OnExecute(T data, FP tick);
    }
}
