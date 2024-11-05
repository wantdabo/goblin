using Animancer;
using UnityEngine;
using Animation = Goblin.Gameplay.Render.Behaviors.Common.Animation;

namespace Goblin.Gameplay.Render.Behaviors
{
    /// <summary>
    /// Animancer 动画播放
    /// </summary>
    public class AnimancerAnimation : Animation
    {
        /// <summary>
        /// Animancer 组件
        /// </summary>
        private NamedAnimancerComponent namedanimancer { get; set; }
        /// <summary>
        /// Animancer 播放状态
        /// </summary>
        private AnimancerState state { get; set; }

        protected override void OnPlay(string name, byte layer = 0)
        {
            state = namedanimancer.TryPlay(name);
            if (null != names[layer] && names[layer].Equals(name)) state.Time = 0f;
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
