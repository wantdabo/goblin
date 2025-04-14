using Animancer;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 动画代理
    /// </summary>
    public class Animation : Agent
    {
        private AnimancerComponent animancer { get; set; }
        private AnimancerState animstate { get; set; }
        private string animname { get; set; }
        private float tarduration { get; set; }
        private float mixduration { get; set; }
        
        protected override void OnReady()
        {
            world.ticker.eventor.Listen<TickEvent>(OnTick);
            animancer = null;
            animstate = null;
        }

        protected override void OnReset()
        {
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
            animancer = null;
            animstate = null;
        }

        public void Play(string animname, float tarduration, float mixduration)
        {
            this.animname = animname;
            this.tarduration = tarduration;
            this.mixduration = mixduration;
        }

        private void OnTick(TickEvent e)
        {
            if (null == animancer)
            {
                var model = world.GetAgent<Model>(actor);
                if (null == model || null == model.go) return;
                animancer = model.go.GetComponent<AnimancerComponent>();
            }

            animstate = animancer.TryPlay(animname, mixduration);
            if (null == animstate) return;
            animstate.Speed = 0;
            animstate.Time = Mathf.Clamp(animstate.Time + e.tick, 0, tarduration);
        }
    }
}