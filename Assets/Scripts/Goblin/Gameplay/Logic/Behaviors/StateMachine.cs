using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 状态机
    /// </summary>
    [Ticking]
    public class StateMachine : Behavior<StateMachineInfo>
    {
        public void AddState(byte state)
        {
            if (info.states.Contains(state)) return;
            info.states.Add(state);
        }

        public void ChangeState(byte state)
        {
            if (info.current == state) return;
            info.frames = 0;
            info.current = state;
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            Automatic();
            info.frames++;
        }

        private void Automatic()
        {
            if (null == info.states) return;
            var next = info.current;
            foreach (var state in info.states)
            {
                if (state == info.current) continue;
                if (false == STATE_DEFINE.PASSES.TryGetValue(info.current, out var passes) || false == passes.Contains(state)) continue;
                if (false == Condition(state)) continue;
                next = state;
            }
            
            ChangeState(next);
        }

        private bool Condition(byte state)
        {
            switch (state)
            {
                case STATE_DEFINE.IDLE:

                    break;
                case STATE_DEFINE.MOVE:

                    break;
                case STATE_DEFINE.JUMP:

                    break;
                case STATE_DEFINE.FALL:

                    break;
                case STATE_DEFINE.CASTING:

                    break;
            }

            return false;
        }
    }
}