using GoblinFramework.Core;
using GoblinFramework.Gameplay.States;

namespace GoblinFramework.Gameplay.Skills
{
    public enum SkillPipelineState
    {
        Start,
        Cost,
        Reading,
        Cast,
        Project,
        Hit,
        Finish
    }

    public class SkillPipelineInfo : StateMachineInfo
    {
        public Actor caster;
    }

    public class SkillPipeline : Behavior<SkillPipelineInfo>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }
    }
}