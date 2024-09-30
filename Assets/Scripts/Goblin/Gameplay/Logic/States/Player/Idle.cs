using Goblin.Gameplay.Logic.Common.StateMachine;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    public class Idle : State
    {
        public override uint id => State.PLAYER_IDLE;
        protected override List<uint> passes => new() { PLAYER_RUN };

        public override bool OnCheck()
        {
            return true;
        }
    }
}
