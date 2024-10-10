using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Skills
{
    public class SkillLauncher : Behavior<Translator>
    {
        private Dictionary<uint, SkillPipeline> spdict = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<FPTickEvent>(OnFPTick);
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
        }

        public SkillPipeline Get(uint id)
        {
            return spdict.GetValueOrDefault(id);
        }

        public void Unload(uint id)
        {
            var sp = Get(id);
            if (null == sp) return;
            spdict.Remove(id);
            sp.Destroy();
        }

        public bool Load(uint id)
        {
            var sp = Get(id);
            if (null != sp) return false;
            
            sp = AddComp<SkillPipeline>();
            sp.launcher = this;
            sp.Create();
            spdict.Add(id, sp);

            return true;
        }
        
        private void OnFPTick(FPTickEvent e)
        {
            foreach (var sp in spdict.Values) sp.OnFPTick(e.tick);
        }
    }
}
