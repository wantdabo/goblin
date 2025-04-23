using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Batches
{
    public class StateMachineBatch : Batch
    {
        private AnimationConfig animcfg { get; set; } = default;
        
        protected override void OnTick(TickEvent e)
        {
            if (false == world.statebucket.GetStates<StateMachineState>(StateType.StateMachine, out var states)) return;
            foreach (var state in states)
            {
                world.statebucket.GetState<TagState>(state.actor, StateType.Tag, out var tagstate);
                
                var modelinfo = engine.cfg.location.ModelInfos.Get(tagstate.tags[TAG_DEFINE.MODEL_ID]);
                if (null == animcfg) animcfg = engine.gameres.location.LoadModelAnimationConfigSync(modelinfo.Animation);
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
            states.Clear();
            ObjectCache.Set(states);
        }
    }
}