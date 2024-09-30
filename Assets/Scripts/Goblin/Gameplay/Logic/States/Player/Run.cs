using Goblin.Gameplay.Logic.Common.StateMachine;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    public class Run : State
    {
        public override uint id => State.PLAYER_RUN;
        protected override List<uint> passes => new() { PLAYER_IDLE };

        public override bool OnCheck()
        {
            return false;
        }
    }
}
