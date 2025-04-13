using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 运动行为
    /// </summary>
    public class Movement : Behavior<MovementInfo>
    {
        protected override void OnAssemble()
        {
            base.OnAssemble();
            actor.eventor.Listen<StateChangedEvent>(actor.eventor, OnStateChanged);
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            actor.eventor.UnListen<StateChangedEvent>(actor.eventor, OnStateChanged);
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="motion">运动数据</param>
        public void Move(FPVector3 motion)
        {
            if (false == actor.SeekBehavior(out StateMachine machine) || false == machine.TryChangeState(STATE_DEFINE.MOVE)) return;
            if (actor.SeekBehaviorInfo(out SpatialInfo spatial))
            {
                spatial.position += motion;
                
                var dire = motion.normalized;
                FP angle = FPMath.Atan2(dire.x, dire.z) * FPMath.Rad2Deg;
                spatial.euler = FPVector3.up * angle;
            }
            
            SetMoving();
        }

        /// <summary>
        /// 标记为运动状态
        /// </summary>
        private void SetMoving()
        {
            info.moving = true;
        }

        protected override void OnEndTick()
        {
            base.OnEndTick();
            // 如果没有在移动状态, 则尝试切换到 Idle 状态
            if (false == info.moving && actor.SeekBehavior(out StateMachine machine))
            {
                machine.TryChangeState(STATE_DEFINE.IDLE);
            }

            info.moving = false;
        }
        
        private void OnStateChanged(StateChangedEvent e)
        {
        }
    }
}