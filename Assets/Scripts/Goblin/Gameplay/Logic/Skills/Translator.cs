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
        /// 技能管线信息列表
        /// </summary>
        private Dictionary<uint, (byte state, uint frame)> pipelineinfos = new();

        protected override void OnRIL()
        {
            foreach (uint id in behavior.skills)
            {
                var pipeline = behavior.Get(id);
                if (null == pipeline) continue;

                if (false == pipelineinfos.TryGetValue(id, out var info))
                {
                    info = (SkillPipelineStateDef.None, 0);
                    pipelineinfos.Add(id, info);
                }
                
                if (pipeline.state != info.state || pipeline.frame != info.frame)
                {
                    pipelineinfos.Remove(id);
                    pipelineinfos.Add(id, (pipeline.state, pipeline.frame));
                    if (SkillPipelineStateDef.None == pipeline.state) return;
                    behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_SKILL_PIPELINE_INFO(pipeline.id, pipeline.state, pipeline.frame, pipeline.length));
                }
            }
        }
    }
}
