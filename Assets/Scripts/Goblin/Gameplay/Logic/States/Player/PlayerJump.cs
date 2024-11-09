using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家跳跃状态
    /// </summary>
    public class PlayerJump : State
    {
        public override uint id => STATE_DEFINE.PLAYER_JUMP;

        protected override List<uint> passes => null;
        
        public override bool OnValid()
        {
            return false;
        }
    }
}
