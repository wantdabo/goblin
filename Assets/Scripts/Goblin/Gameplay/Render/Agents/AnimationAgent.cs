using Animancer;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;
using UnityEngine;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 动画代理
    /// </summary>
    public class AnimationAgent : Agent
    {
        private AnimationConfig animcfg { get; set; } = default;
        private string animcfgName { get; set; } = default;
        private AnimancerComponent animancer { get; set; }
        private AnimancerState animstate { get; set; }
        private string animname { get; set; }
        private float tarduration { get; set; }
        private float mixduration { get; set; }
        
        protected override void OnReady()
        {
            animancer = null;
            animstate = null;
            
            CareState<StateMachineState>((state) =>
            {
                StateConv2AnimData(state);
                if (null == animname) return;
                Play(animname, tarduration, mixduration);
            });
        }

        protected override void OnReset()
        {
            animancer = null;
            animstate = null;
        }

        private void StateConv2AnimData(StateMachineState state)
        {
            
            if (false == world.statebucket.SeekState<TagState>(state.actor, out var tagstate) || false == tagstate.tags.TryGetValue(TAG_DEFINE.MODEL, out var model)) return;
            var modelinfo = world.engine.cfg.location.ModelInfos.Get(model);
            
            if (null == animcfgName || false == modelinfo.Animation.Equals(animcfgName))
            {
                animcfgName = modelinfo.Animation;
                if (null != animcfg) GameObject.Destroy(animcfg);
                animcfg = world.engine.gameres.location.LoadModelAnimationConfigSync(animcfgName);
            }

                
            var animinfo = animcfg.GetAnimationInfo(state.current);
            if (null == animinfo) return;
                
            animname = animinfo.name;
            mixduration = animinfo.mixduration;
            tarduration = state.elapsed * Config.Int2Float;

            var beforeAnimInfo = animinfo.GetMixAnimationInfo(state.last);
            if (null != beforeAnimInfo && tarduration < beforeAnimInfo.duration)
            {
                animname = beforeAnimInfo.name;
                mixduration = beforeAnimInfo.mixduration;
            }
        }

        public void Play(string animname, float tarduration, float mixduration)
        {
            this.animname = animname;
            this.tarduration = tarduration;
            this.mixduration = mixduration;
        }

        protected override void OnFlash()
        {
            base.OnFlash();
            if (null == animstate) return;
            animstate.Time = tarduration;
        }

        protected override void OnChase(float tick, float timescale)
        {
            base.OnChase(tick, timescale);
            if (null == animancer)
            {
                var model = world.GetAgent<ModelAgent>(actor);
                if (null == model || null == model.go) return;
                animancer = model.go.GetComponent<AnimancerComponent>();
            }

            if (null == animancer) return;
            animstate = animancer.TryPlay(animname, mixduration * (1 / timescale));
            if (null == animstate) return;
            animstate.Speed = 0;
            animstate.Time = Mathf.Clamp(animstate.Time + tick * timescale, 0, tarduration);
        }
    }
}