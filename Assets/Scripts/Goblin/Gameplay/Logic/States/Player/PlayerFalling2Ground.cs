using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Physics;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家跳跃结束状态
    /// </summary>
    public class PlayerFalling2Ground : State
    {
        public override uint id => STATE_DEFINE.PLAYER_FALLING2GROUND;
        
        protected override List<uint> passes => new() { STATE_DEFINE.PLAYER_IDLE, STATE_DEFINE.PLAYER_RUN, STATE_DEFINE.PLAYER_HURT, STATE_DEFINE.PLAYER_ATTACK };
        
        private PhysAgent physagent { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            physagent = machine.paramachine.actor.GetBehavior<PhysAgent>();
        }
        
        public override bool OnValid()
        {
            return physagent.grounded;
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            physagent.LossForce();
        }
    }
}
