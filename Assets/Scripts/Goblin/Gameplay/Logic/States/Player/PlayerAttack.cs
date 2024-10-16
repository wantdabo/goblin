using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Skills;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    public class PlayerAttack : State
    {
        public override uint id => StateDef.PLAYER_ATTACK;
        protected override List<uint> passes => null;
        private uint skillid { get; set; }
        private Gamepad gamepad { get; set; }
        private SkillLauncher launcher { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            machine.paramachine.actor.eventor.Listen<SkillPipelineStateEvent>(OnSkillPipelineStateEvent);
            
            gamepad = machine.paramachine.actor.GetBehavior<Gamepad>();
            launcher = machine.paramachine.actor.GetBehavior<SkillLauncher>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            machine.paramachine.actor.eventor.UnListen<SkillPipelineStateEvent>(OnSkillPipelineStateEvent);
        }

        public override bool OnCheck()
        {
            var ba = gamepad.GetInput(InputType.BA);
            // TODO 新增 COMBO 之类的判定
            return ba.release;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            skillid = 10001;
            launcher.Launch(skillid);
        }
        
        private void OnSkillPipelineStateEvent(SkillPipelineStateEvent e)
        {
            if (e.id == skillid && e.state == SPStateDef.End)
            {
                Break();
            }
        }
    }
}
