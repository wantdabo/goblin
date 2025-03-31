using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 状态机
    /// </summary>
    public class StateMachine : Behavior<StateMachineInfo>
    {
        public bool TryChangeState(byte state)
        {
            if (info.current == state) return true;
            if (false == QueryPassState(state)) return false;

            ChangeState(state);

            return true;
        }
        
        public void ChangeState(byte state)
        {
            info.frames = 0;
            info.current = state;
        }
        
        private bool QueryPassState(byte state)
        {
            if (STATE_DEFINE.PASSES.TryGetValue(state, out var passes) && passes.Contains(info.current))
            {
                return true;
            }

            return false;
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            info.frames++;
        }
    }
}