using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

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
            var states = world.statebucket.GetStates<StateMachineState>(StateType.StateMachine);
            if (null == states) return;
            foreach (var state in states)
            {
                var tagstate = world.statebucket.GetState<TagState>(state.actor, StateType.Tag);
                
                var modelinfo = engine.cfg.location.ModelInfos.Get(tagstate.model);
                var animcfg = engine.gameres.location.LoadModelAnimationConfigSync(modelinfo.Animation);
                if (null == animcfg) continue;
                
                var animinfo = animcfg.GetAnimationInfo(state.current);
                if (null == animinfo) continue;
                
                string animname = animinfo.name;
                var tarduration = state.elapsed * Config.Int2Float;
                var mixduration = animinfo.mixduration;
                
                var beforeAnimInfo = animinfo.GetMixAnimationInfo(state.last);
                if (null != beforeAnimInfo && tarduration < beforeAnimInfo.duration)
                {
                    animname = beforeAnimInfo.name;
                    mixduration = beforeAnimInfo.mixduration;
                }
                
                if (null == animname) continue;
                var animation = world.EnsureAgent<AnimationAgent>(state.actor);
                animation.Play(animname, tarduration, mixduration);
            }
        }
    }
}