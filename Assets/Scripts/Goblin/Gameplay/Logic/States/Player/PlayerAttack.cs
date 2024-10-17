using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Skills;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.States.Player
{
    public class PlayerAttack : State
    {
        public override uint id => StateDef.PLAYER_ATTACK;
        protected override List<uint> passes => null;
        private SkillLauncher launcher { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            launcher = machine.paramachine.actor.GetBehavior<SkillLauncher>();
        }

        public override bool OnCheck()
        {
            return launcher.launchskill.playing;
        }

        public override void OnTick(uint frame, FP tick)
        {
            base.OnTick(frame, tick);
            if (launcher.launchskill.playing) return;
            Break();
        }
    }
}
