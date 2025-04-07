using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// Gamepad/手柄
    /// </summary>
    public class Gamepad : Behavior<GamepadInfo>
    {
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            var joystick = GetInput(InputType.Joystick);
            if (joystick.press)
            {
                if (actor.SeekBehavior(out Movement movement) && actor.SeekBehaviorInfo(out AttributeInfo attribute))
                {
                    var motion = joystick.dire.normalized * attribute.moveseed * tick;
                    movement.Move(new FPVector3(motion.x, 0, motion.y));
                }
            }
        }

        protected override void OnEndTick()
        {
            ClearReleaseTokenAll();
        }
        
        /// <summary>
        /// 获取手柄的某个按键状态
        /// </summary>
        /// <param name="inputType">按键类型</param>
        /// <returns>按键数据</returns>
        /// <exception cref="Exception">未找到该类型按键的数据</exception>
        public InputInfo GetInput(InputType inputType)
        {
            if (info.inputdict.TryGetValue(inputType, out InputInfo input)) return input;

            throw new Exception($"inputType not found {inputType}");
        }

        /// <summary>
        /// 设置按键数据
        /// </summary>
        /// <param name="inputType">按键类型</param>
        /// <param name="input">按键数据</param>
        public void SetInput(InputType inputType, InputInfo input)
        {
            if (info.inputdict.TryGetValue(inputType, out var oldInput))
            {
                // 上一次是 press，新的是 unpress 表示技能释放出去了。
                if (oldInput.press && false == input.press) input.release = true;
                info.inputdict.Remove(inputType);
            }

            info.inputdict.Add(inputType, input);
        }

        /// <summary>
        /// 清理所有按键松发状态
        /// </summary>
        public void ClearReleaseTokenAll()
        {
            ClearReleaseToken(InputType.Joystick);
            ClearReleaseToken(InputType.BA);
            ClearReleaseToken(InputType.BB);
            ClearReleaseToken(InputType.BC);
            ClearReleaseToken(InputType.BD);
        }

        /// <summary>
        /// 清理按键松发状态
        /// </summary>
        /// <param name="inputType">按键类型</param>
        public void ClearReleaseToken(InputType inputType)
        {
            var input = GetInput(inputType);
            input.release = false;
            info.inputdict.Remove(inputType);
            info.inputdict.Add(inputType, input);
        }
    }
}