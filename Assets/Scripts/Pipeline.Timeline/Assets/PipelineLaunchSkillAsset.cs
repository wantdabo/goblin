using System.ComponentModel;
using Animancer;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Pipeline.Timeline.Assets.Common;
using UnityEngine;
using UnityEngine.Playables;

namespace Pipeline.Timeline.Assets
{
    [DisplayName("释放技能指令")]
    public class PipelineLaunchSkillAsset : PipelineAsset<PipelineLaunchSkillAsset.PipelineLaunchSkillBehavior, LaunchSkillData>
    {
        public class PipelineLaunchSkillBehavior : PipelineBehavior<LaunchSkillData>
        {
        }
    }
}