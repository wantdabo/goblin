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
        /// <param name="tick">步长</param>
        public void Move(FPVector3 dire, FP tick)
        {
            if (false == actor.SeekBehavior(out StateMachine machine)) return;
            if (false == machine.TryChangeState(STATE_DEFINE.MOVE)) return;
            if (false == actor.SeekBehaviorInfo(out AttributeInfo attribute)) return;
            if (false == actor.SeekBehaviorInfo(out SpatialInfo spatial)) return;
            
            info.turnmove = true;
            info.motion = MOVEMENT_DEFINE.MOVE;
            
            dire = dire.normalized;
            var motion = dire * attribute.movespeed * tick;
            spatial.position += motion;

            FP angle = FPMath.Atan2(dire.x, dire.z) * FPMath.Rad2Deg;
            spatial.euler = FPVector3.up * angle;
        }

        /// <summary>
        /// 传送
        /// </summary>
        /// <param name="position">位置</param>
        public void Transport(FPVector3 position)
        {
            if (false == actor.SeekBehaviorInfo(out SpatialInfo spatial)) return;
            info.motion = MOVEMENT_DEFINE.TRANSPORT;
            spatial.position = position;
        }

        /// <summary>
        /// 管线
        /// </summary>
        /// <param name="position">位置</param>
        public void Flow(FPVector3 position)
        {
            if (false == actor.SeekBehaviorInfo(out SpatialInfo spatial)) return;
            info.motion = MOVEMENT_DEFINE.FLOW;
            spatial.position = position;
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
            if (false == info.turnmove && actor.SeekBehavior(out StateMachine machine))
            {
                machine.TryChangeState(STATE_DEFINE.IDLE);
            }
            info.turnmove = false;
        }
    }
}