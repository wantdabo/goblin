using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using System;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.Inputs
{
    /// <summary>
    /// 按键输入的枚举
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// 摇杆
        /// </summary>
        Joystick,
    }

    /// <summary>
    /// 按键输入的结构体
    /// </summary>
    public struct InputInfo
    {
        /// <summary>
        /// 摁下之后 -> TRUE
        /// </summary>
        public bool press;
        /// <summary>
        /// 摁下之后，抬起 -> TRUE
        /// </summary>
        public bool release;
        /// <summary>
        /// 按键的方向
        /// </summary>
        public TSVector2 dire;
    }

    /// <summary>
    /// Gamepad/手柄
    /// </summary>
    public class Gamepad : Behavior
    {
        private Dictionary<InputType, InputInfo> InputDict = new()
        {
            { InputType.Joystick, new InputInfo { press = false, dire = TSVector2.zero } },
        };

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<FPLateTickEvent>(OnFPLateTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<FPLateTickEvent>(OnFPLateTick);
        }

        /// <summary>
        /// 获取手柄的某个按键状态
        /// </summary>
        /// <param name="inputType">按键类型</param>
        /// <returns>按键数据</returns>
        /// <exception cref="Exception">未找到该类型按键的数据</exception>
        public InputInfo GetInput(InputType inputType)
        {
            if (InputDict.TryGetValue(inputType, out InputInfo input)) return input;

            throw new Exception($"inputType not found {inputType}");
        }

        /// <summary>
        /// 设置按键数据
        /// </summary>
        /// <param name="inputType">按键类型</param>
        /// <param name="input">按键数据</param>
        public void SetInput(InputType inputType, InputInfo input)
        {
            if (InputDict.TryGetValue(inputType, out var oldInput))
            {
                // 上一次是 press，新的是 unpress 表示技能释放出去了。
                if (oldInput.press && false == input.press) input.release = true;
                InputDict.Remove(inputType);
            }

            InputDict.Add(inputType, input);
        }

        private void OnFPLateTick(FPLateTickEvent e)
        {
            ClearReleaseTokenAll();
        }

        /// <summary>
        /// 清理所有按键松发状态
        /// </summary>
        public void ClearReleaseTokenAll()
        {
            ClearReleaseToken(InputType.Joystick);
        }

        /// <summary>
        /// 清理按键松发状态
        /// </summary>
        /// <param name="inputType">按键类型</param>
        public void ClearReleaseToken(InputType inputType)
        {
            var input = GetInput(inputType);
            input.release = false;
            InputDict.Remove(inputType);
            InputDict.Add(inputType, input);
        }
    }
}