using Goblin.Core;
using Goblin.Gameplay.Common.SkillDatas.Common;
using Goblin.Gameplay.Logic.Skills.ActionCache.Common;
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
        /// <param name="cache">缓存</param>
        public void Enter(SkillActionData data, SkillActionCache cache)
        {
            OnEnter(data, cache);
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cache">缓存</param>
        /// <param name="frame">执行帧号</param>
        /// <param name="tick">tick</param>
        public void Execute(SkillActionData data, SkillActionCache cache, uint frame, FP tick)
        {
            OnExecute(data, cache, frame, tick);
        }

        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cache">缓存</param>
        public void Exit(SkillActionData data, SkillActionCache cache)
        {
            OnExit(data, cache);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cache">缓存</param>
        protected abstract void OnEnter(SkillActionData data, SkillActionCache cache);
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cache">缓存</param>
        /// <param name="frame">执行帧号</param>
        /// <param name="tick">tick</param>
        protected abstract void OnExecute(SkillActionData data, SkillActionCache cache, uint frame, FP tick);
        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cache">缓存</param>
        protected abstract void OnExit(SkillActionData data, SkillActionCache cache);
    }

    /// <summary>
    /// 技能管线行为
    /// </summary>
    /// <typeparam name="TD">技能管线行为数据类型</typeparam>
    /// <typeparam name="TC">技能管线行为缓存类型</typeparam>
    public abstract class SkillAction<TD, TC> : SkillAction where TD : SkillActionData where TC : SkillActionCache
    {
        protected override void OnEnter(SkillActionData data, SkillActionCache cache)
        {
            OnEnter((TD)data, (TC)cache);
        }

        protected override void OnExecute(SkillActionData data, SkillActionCache cache, uint frame, FP tick)
        {
            OnExecute((TD)data, (TC)cache, frame, tick);
        }

        protected override void OnExit(SkillActionData data, SkillActionCache cache)
        {
            OnExit((TD)data, (TC)cache);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cache">缓存</param>
        protected virtual void OnEnter(TD data, TC cache) { }
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cache">缓存</param>
        /// <param name="frame">执行帧号</param>
        /// <param name="tick">tick</param>
        protected abstract void OnExecute(TD data, TC cache, uint frame, FP tick);
        /// <summary>
        /// 离开
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="cache">缓存</param>
        protected virtual void OnExit(TD data, TC cache) { }
    }
}
