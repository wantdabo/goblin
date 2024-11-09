using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家死亡状态
    /// </summary>
    public class PlayerDeath : State
    {
        public override uint id => STATE_DEFINE.PLAYER_DEATH;

        protected override List<uint> passes => null;
        
        public override bool OnValid()
        {
            return false;
        }
    }
}
