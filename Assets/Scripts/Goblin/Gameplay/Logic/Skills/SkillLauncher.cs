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
        public SkillNavigation nav { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
            nav = AddComp<SkillNavigation>();
            nav.launcher = this;
            nav.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<FPTickEvent>(OnFPTick);
        }

        public void Launch(uint id)
        {
            var pipeline = Get(id);
            if (null == pipeline) return;
            pipeline.Launch();
        }

        public SkillPipeline Get(uint id)
        {
            return skilldict.GetValueOrDefault(id);
        }

        public void Load(uint id)
        {
            var sp = Get(id);
            if (null != sp) return;

            sp = AddComp<SkillPipeline>();
            sp.id = id;
            sp.launcher = this;
            sp.Create();
            skills.Add(id);
            skilldict.Add(id, sp);
        }

        private void OnFPTick(FPTickEvent e)
        {
            foreach (var sp in skilldict.Values) sp.OnFPTick(e.tick);
        }
    }
}
