using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Common.StateMachine;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家受击状态
    /// </summary>
    public class PlayerHurt : State
    {
        public override uint id => StateDef.PLAYER_HURT;
        protected override List<uint> passes => null;

        private bool suffer = false;
        private FP elapsed = 0;
        private FP duration = FP.EN1 * 5;

        protected override void OnCreate()
        {
            base.OnCreate();
            machine.paramachine.actor.eventor.Listen<RecvHurtEvent>(OnRecvHurt);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            machine.paramachine.actor.eventor.UnListen<RecvHurtEvent>(OnRecvHurt);
        }

        public override bool OnCheck()
        {
            return suffer;
        }

        public override void OnTick(uint frame, FP tick)
        {
            base.OnTick(frame, tick);
            elapsed += tick;
            if (elapsed >= duration)
            {
                elapsed = 0;
                suffer = false;
                Break();
            }
        }

        private void OnRecvHurt(RecvHurtEvent e)
        {
            suffer = true;
        }
    }
}
