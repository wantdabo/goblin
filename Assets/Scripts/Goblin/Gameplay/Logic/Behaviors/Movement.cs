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
        /// <param name="dire">运动数据</param>
        /// <param name="tick">tick</param>
        public void Move(FPVector3 dire, FP tick)
        {
            if (false == actor.SeekBehavior(out StateMachine machine)) return;
            if (false == actor.SeekBehaviorInfo(out AttributeInfo attribute)) return;
            if (false == machine.TryChangeState(STATE_DEFINE.MOVE)) return;
            if (false == actor.SeekBehaviorInfo(out SpatialInfo spatial)) return;

            dire = dire.normalized;
            var motion = dire * attribute.movespeed * tick;
            spatial.position += motion;

            FP angle = FPMath.Atan2(dire.x, dire.z) * FPMath.Rad2Deg;
            spatial.euler = FPVector3.up * angle;

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
            if (false == actor.SeekBehavior(out Gamepad gamepad)) return;
            
            var joystick = gamepad.GetInput(INPUT_DEFINE.JOYSTICK);
            if (false == joystick.press) return;
            
            Move(new FPVector3(joystick.dire.x, 0, joystick.dire.y), tick);
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