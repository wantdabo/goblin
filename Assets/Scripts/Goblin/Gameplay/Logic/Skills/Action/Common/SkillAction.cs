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

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data"></param>
        public void Enter(SkillActionData data)
        {
            OnEnter(data);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="frame">执行帧号</param>
        /// <param name="tick">tick</param>
        public void Execute(SkillActionData data, uint frame, FP tick)
        {
            OnExecute(data, frame, tick);
        }

        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="data">数据</param>
        public void Exit(SkillActionData data)
        {
            OnExit(data);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data">数据</param>
        protected abstract void OnEnter(SkillActionData data);
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="frame">执行帧号</param>
        /// <param name="tick">tick</param>
        protected abstract void OnExecute(SkillActionData data, uint frame, FP tick);
        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="data">数据</param>
        protected abstract void OnExit(SkillActionData data);
    }

    /// <summary>
    /// 技能管线行为
    /// </summary>
    /// <typeparam name="T">技能管线行为数据类型</typeparam>
    public abstract class SkillAction<T> : SkillAction where T : SkillActionData
    {
        protected override void OnEnter(SkillActionData data)
        {
            OnEnter((T)data);
        }

        protected override void OnExecute(SkillActionData data, uint frame, FP tick)
        {
            OnExecute((T)data, frame, tick);
        }

        protected override void OnExit(SkillActionData data)
        {
            OnExit((T)data);
        }
        
        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data">数据</param>
        protected virtual void OnEnter(T data) { }
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="frame">执行帧号</param>
        /// <param name="tick">tick</param>
        protected abstract void OnExecute(T data, uint frame, FP tick);
        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="data">数据</param>
        protected virtual void OnExit(T data) { }
    }
}
