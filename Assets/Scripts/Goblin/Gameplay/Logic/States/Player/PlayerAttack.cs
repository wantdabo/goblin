﻿using Goblin.Common;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Skills;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家攻击状态
    /// </summary>
    public class PlayerAttack : State
    {
        public override uint id => STATE_DEFINE.PLAYER_ATTACK;

        protected override List<uint> passes => new() { STATE_DEFINE.PLAYER_HURT };

        private SkillLauncher launcher { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            launcher = machine.paramachine.actor.GetBehavior<SkillLauncher>();
        }

        public override bool OnValid()
        {
            return launcher.launching.playing;
        }

        public override void OnExit()
        {
            base.OnExit();
            if (launcher.launching.playing)
            {
                var pipeline = launcher.Get(launcher.launching.skill);
                if (null != pipeline) pipeline.Break();
            }
        }

        public override void OnTick(uint frame, FP tick)
        {
            base.OnTick(frame, tick);
            if (launcher.launching.playing) return;
            Break();
        }
    }
}
