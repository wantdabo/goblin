using Goblin.Core;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills
{
    /// <summary>
    /// 技能管线状态
    /// </summary>
    public enum SkillPipelineState
    {
        /// <summary>
        /// 打断
        /// </summary>
        Break,
        /// <summary>
        /// 释放
        /// </summary>
        Start,
        /// <summary>
        /// 释放中
        /// </summary>
        Casting,
        /// <summary>
        /// 结束
        /// </summary>
        End
    }

    /// <summary>
    /// 技能管线
    /// </summary>
    public class SkillPipeline : Comp
    {
        /// <summary>
        /// 技能状态
        /// </summary>
        public SkillPipelineState state { get; private set; } = SkillPipelineState.End;
        /// <summary>
        /// 技能发射器
        /// </summary>
        public SkillLauncher launcher { get; set; }
        /// <summary>
        /// 当前执行帧号
        /// </summary>
        public uint frame { get; private set; }

        public void Break()
        {
            state = SkillPipelineState.Break;
            OnBreak();
        }

        public void Launch()
        {
            if (SkillPipelineState.End != state) return;
            state = SkillPipelineState.Start;
            OnStart();
        }

        private void OnBreak()
        {
            // TODO Break 部分
        }

        private void OnStart()
        {
            // TODO Start 部分
            state = SkillPipelineState.Casting;
        }

        private void OnCasting(FP tick)
        {
            // TODO Casting 部分
            // TODO 检测到 End 信号
            state = SkillPipelineState.End;
            OnEnd();
        }
        
        private void OnEnd()
        {
        }

        public void OnFPTick(FP tick)
        {
            if (SkillPipelineState.Casting != state) return;
            OnCasting(tick);
            // TODO 驱动技能
        }
    }
}
