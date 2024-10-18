using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Skills
{
    public class SkillLauncher : Behavior<Translator>
    {
        public List<uint> skills { get; private set; } = new();
        private Dictionary<uint, SkillPipeline> skilldict = new();
        public SkillCaster caster { get; private set; }
        /// <summary>
        /// 技能释放中
        /// </summary>
        public (bool playing, uint skill) launchskill
        {
            get
            {
                foreach (uint skill in skills)
                {
                    var pipeline = Get(skill);
                    if (SkillPipelineStateDef.None != pipeline.state) return (true, skill);
                }

                return (false, default);
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            caster = AddComp<SkillCaster>();
            caster.launcher = this;
            caster.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
        }

        public bool Launch(uint id)
        {
            var pipeline = Get(id);
            if (null == pipeline) return false;

            return pipeline.Launch();
        }

        public SkillPipeline Get(uint id)
        {
            return skilldict.GetValueOrDefault(id);
        }

        public void Load(uint id)
        {
            var pipeline = Get(id);
            if (null != pipeline) return;

            pipeline = AddComp<SkillPipeline>();
            pipeline.id = id;
            pipeline.launcher = this;
            pipeline.Create();
            skills.Add(id);
            skilldict.Add(id, pipeline);
        }

        private void OnFPTick(FPTickEvent e)
        {
            caster.OnFPTick(e.tick);
            foreach (var pipeline in skilldict.Values) pipeline.OnFPTick(e.tick);
        }
    }
}
