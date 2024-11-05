using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Logic.Spatials;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.States.Player
{
    /// <summary>
    /// 玩家受击状态
    /// </summary>
    public class PlayerHurt : State
    {
        public override uint id => STATE_DEFINE.PLAYER_HURT;
        protected override List<uint> passes => null;
        
        private Spatial spatial { get; set; }
        private bool suffer = false;
        private FP elapsed = 0;
        private FP duration = FP.EN1 * 20;

        protected override void OnCreate()
        {
            base.OnCreate();
            machine.paramachine.actor.eventor.Listen<RecvHurtEvent>(OnRecvHurt);
            spatial = machine.paramachine.actor.GetBehavior<Spatial>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            machine.paramachine.actor.eventor.UnListen<RecvHurtEvent>(OnRecvHurt);
        }

        public override bool OnValid()
        {
            return suffer;
        }

        public override void OnExit()
        {
            base.OnExit();
            elapsed = 0;
            suffer = false;
        }

        public override void OnTick(uint frame, FP tick)
        {
            base.OnTick(frame, tick);
            elapsed += tick;
            if (elapsed >= duration)
            {
                Break();
            }
        }

        private void OnRecvHurt(RecvHurtEvent e)
        {
            if (null != machine.current && id == machine.current.id)
            {
                Break();
                OnRecvHurt(e);
                
                return;
            }

            suffer = true;
            var target = machine.paramachine.actor.stage.GetActor(e.from);
            if (null == target || false == target.live.alive) return;
            var tspatial = target.GetBehavior<Spatial>();

            if (tspatial.position == spatial.position) return;
            
            var dire = (tspatial.position - spatial.position).normalized;
            spatial.position -= dire * FP.EN2 * 2;
            spatial.rotation = TSQuaternion.LookRotation(dire);
        }
    }
}
