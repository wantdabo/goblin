using Animancer;
using UnityEngine;
using Animation = Goblin.Gameplay.Render.Behaviors.Common.Animation;

namespace Goblin.Gameplay.Render.Behaviors
{
    public class AnimancerAnimation : Animation
    {
        private NamedAnimancerComponent namedanimancer { get; set; }
        private AnimancerState state { get; set; }

        protected override void OnPlay(string name, byte layer = 0)
        {
            state = namedanimancer.TryPlay(name, 0.05f, FadeMode.NormalizedDuration);
            state.Speed = 0f;
        }

        protected override void OnTick(float tick)
        {
            if (null == state) return;
            if (lerp)
            {
                var t = state.Time / state.Length;
                if (t >= lerpt) state.Time = state.Length * lerpt;
                state.Time = Mathf.Clamp(state.Time + tick, 0, state.Length);
                
                return;
            }
            
            state.Time += tick;
        }

        protected override bool OnNextSequeue()
        {
            if (null == state) return true;
            if (null != state && false == state.IsLooping) return state.Time > state.Length;

            return base.OnNextSequeue();
        }

        protected override void OnModelChanged(GameObject go)
        {
            namedanimancer = go.GetComponentInChildren<NamedAnimancerComponent>();
            namedanimancer.UpdateMode = AnimatorUpdateMode.Normal;
        }
    }
}
