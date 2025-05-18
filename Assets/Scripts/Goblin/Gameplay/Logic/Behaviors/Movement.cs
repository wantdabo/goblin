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
            
            MarkMoving();
        }

        /// <summary>
        /// 标记为运动状态
        /// </summary>
        private void MarkMoving()
        {
            info.moving = true;
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            
            if (actor.SeekBehavior(out Gamepad gamepad))
            {
                var joystick = gamepad.GetInput(INPUT_DEFINE.JOYSTICK);
                if (joystick.press)
                {
                    if (false == actor.SeekBehavior(out StateMachine statemachine)) return;
                    if (false == statemachine.TryChangeState(STATE_DEFINE.MOVE)) return;
                    
                    if (actor.SeekBehaviorInfo(out AttributeInfo attribute))
                    {
                        var motion = joystick.dire.normalized * attribute.movespeed * tick;
                        Move(new FPVector3(motion.x, 0, motion.y));
                    }
                }
            }
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
    }
}