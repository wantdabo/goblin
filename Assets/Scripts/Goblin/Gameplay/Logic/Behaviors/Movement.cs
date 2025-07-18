using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Flows.Defines;
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
        /// <param name="dire">方向</param>
        /// <param name="tick">步长</param>
        public void Move(FPVector3 dire, FP tick)
        {
            if (false == stage.SeekBehavior(actor, out StateMachine machine)) return;
            if (false == machine.TryChangeState(STATE_DEFINE.MOVE)) return;
            if (false == stage.SeekBehaviorInfo(actor, out AttributeInfo attribute)) return;
            if (false == stage.SeekBehaviorInfo(actor, out SpatialInfo spatial)) return;
            
            dire.Normalize();
            var motion = dire * stage.attrc.GetAttributeValue(attribute, ATTRIBUTE_DEFINE.MOVESPEED) * tick;
            spatial.position += motion;

            FP angle = FPMath.Atan2(dire.x, dire.z) * FPMath.Rad2Deg;
            spatial.euler = FPVector3.up * angle;
            
            info.turnmotion = true;
        }

        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehavior(actor, out Gamepad gamepad)) return;
            
            var joystick = gamepad.GetInput(INPUT_DEFINE.JOYSTICK);
            if (false == joystick.press) return;
            
            Move(new FPVector3(joystick.dire.x, 0, joystick.dire.y), tick);
        }

        protected override void OnEndTick()
        {
            base.OnEndTick();
            // 如果没有在移动状态, 则尝试切换到 Idle 状态
            if (false == info.turnmotion && stage.SeekBehavior(actor, out StateMachine machine))
            {
                machine.TryChangeState(STATE_DEFINE.IDLE);
            }
            info.turnmotion = false;
        }
    }
}