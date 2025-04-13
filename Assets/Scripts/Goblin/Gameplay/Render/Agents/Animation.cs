using Animancer;
using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Agents
{
    /// <summary>
    /// 动画代理
    /// </summary>
    public class Animation : Agent
    {
        private AnimancerComponent animancer { get; set; }
        private AnimancerState animancerState { get; set; }
        
        protected override void OnReady()
        {
            animancer = null;
            world.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnReset()
        {
            animancer = null;
            world.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        private void OnTick(TickEvent e)
        {
            if (null == animancer)
            {
                var model = world.GetAgent<Model>(actor);
                if (null == model || null == model.go) return;
                animancer = model.go.GetComponent<AnimancerComponent>();
            }

            var state = world.summary.GetState(actor, RIL_DEFINE.STATE_MACHINE);
            var statemachine = (RIL_STATE_MACHINE)state.ril;
            switch (statemachine.current)
            {
                case STATE_DEFINE.IDLE:
                    animancerState = animancer.TryPlay("Avatar_Female_Size02_Unagi_Ani_Idle");
                    break;
                case STATE_DEFINE.MOVE:
                    animancerState = animancer.TryPlay("Avatar_Female_Size02_Unagi_Ani_Run");
                    break;
            }

            animancerState.Speed = 0;
            UnityEngine.Debug.Log(statemachine);
        }
    }
}