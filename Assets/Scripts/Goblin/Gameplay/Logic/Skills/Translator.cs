using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Skills
{
    /// <summary>
    /// 技能释放器翻译
    /// </summary>
    public class Translator : Translator<SkillLauncher>
    {
        /// <summary>
        /// 技能管线信息字典
        /// </summary>
        private Dictionary<uint, (byte state, uint frame)> pipelinedict { get; set; } = new();

        protected override void OnRIL()
        {
            foreach (uint id in behavior.skills)
            {
                var pipeline = behavior.Get(id);
                if (null == pipeline) continue;

                if (false == pipelinedict.TryGetValue(id, out var info))
                {
                    info = (SKILL_PIPELINE_STATE_DEFINE.None, 0);
                    pipelinedict.Add(id, info);
                }

                if (pipeline.state != info.state || pipeline.frame != info.frame)
                {
                    pipelinedict.Remove(id);
                    pipelinedict.Add(id, (pipeline.state, pipeline.frame));
                    if (SKILL_PIPELINE_STATE_DEFINE.None == pipeline.state) return;
                    behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_SKILL_PIPELINE_INFO(pipeline.id, pipeline.state, pipeline.frame, pipeline.length));
                }
            }
        }
    }
}
