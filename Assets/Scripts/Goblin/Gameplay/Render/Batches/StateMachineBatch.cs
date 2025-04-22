using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Batches
{
    public class StateMachineBatch : Batch
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            world.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
        }
        
        private void OnTick(TickEvent e)
        {
            var statebundles = world.statebucket.GetStateBundles(RIL_DEFINE.STATE_MACHINE);
            if (null == statebundles) return;
            foreach (var bundle in statebundles)
            {
                var tagstate = world.statebucket.GetState(bundle.actor, RIL_DEFINE.TAG);
                var tag = (RIL_TAG)tagstate.ril;
                
                var modelinfo = engine.cfg.location.ModelInfos.Get(tag.model);
                var animcfg = engine.gameres.location.LoadModelAnimationConfigSync(modelinfo.Animation);
                if (null == animcfg) continue;
                
                var statemachine = (RIL_STATE_MACHINE)bundle.ril;
                var animinfo = animcfg.GetAnimationInfo(statemachine.current);
                if (null == animinfo) continue;
                
                string animname = animinfo.name;
                var tarduration = statemachine.elapsed * Config.Int2Float;
                var mixduration = animinfo.mixduration;
                
                var beforeAnimInfo = animinfo.GetMixAnimationInfo(statemachine.last);
                if (null != beforeAnimInfo && tarduration < beforeAnimInfo.duration)
                {
                    animname = beforeAnimInfo.name;
                    mixduration = beforeAnimInfo.mixduration;
                }
                
                if (null == animname) continue;
                var animation = world.EnsureAgent<AnimationAgent>(bundle.actor);
                animation.Play(animname, tarduration, mixduration);
            }
        }
    }
}