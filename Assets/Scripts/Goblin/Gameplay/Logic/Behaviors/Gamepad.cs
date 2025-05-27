using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Flows.Defines;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// Gamepad/手柄
    /// </summary>
    public class Gamepad : Behavior<GamepadInfo>
    {
        /// <summary>
        /// 获取手柄的某个按键状态
        /// </summary>
        /// <param name="inputType">按键类型</param>
        /// <returns>按键数据</returns>
        /// <exception cref="Exception">未找到该类型按键的数据</exception>
        public InputInfo GetInput(ushort inputType)
        {
            if (info.inputdict.TryGetValue(inputType, out InputInfo input)) return input;

            throw new Exception($"inputType not found {inputType}");
        }

        /// <summary>
        /// 设置按键数据
        /// </summary>
        /// <param name="inputType">按键类型</param>
        /// <param name="press">摁下之后 -> TRUE</param>
        /// <param name="dire">按键的方向</param>
        public void SetInput(ushort inputType, bool press, FPVector2 dire)
        {
            var input = new InputInfo
            {
                press = press,
                release = false,
                dire = dire
            };
            
            if (info.inputdict.TryGetValue(inputType, out var oldInput))
            {
                // 上一次是 press，新的是 unpress 表示技能释放出去了。
                if (oldInput.press && false == input.press) input.release = true;
                info.inputdict.Remove(inputType);
            }

            info.inputdict.Add(inputType, input);
        }
        
        protected override void OnEndTick()
        {
            ClearReleaseTokenAll();
        }

        /// <summary>
        /// 清理所有按键松发状态
        /// </summary>
        private void ClearReleaseTokenAll()
        {
            ClearReleaseToken(INPUT_DEFINE.JOYSTICK);
            ClearReleaseToken(INPUT_DEFINE.BA);
            ClearReleaseToken(INPUT_DEFINE.BB);
            ClearReleaseToken(INPUT_DEFINE.BC);
            ClearReleaseToken(INPUT_DEFINE.BD);
        }

        /// <summary>
        /// 清理按键松发状态
        /// </summary>
        /// <param name="inputType">按键类型</param>
        private void ClearReleaseToken(ushort inputType)
        {
            var input = GetInput(inputType);
            input.release = false;
            info.inputdict.Remove(inputType);
            info.inputdict.Add(inputType, input);
        }
    }
}