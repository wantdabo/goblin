using Goblin.Common;
using System;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Common
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
    /// 按键状态改变
    /// </summary>
    public struct InputStateChangedEvent : IEvent
    {
        /// <summary>
        /// 改变按键
        /// </summary>
        public InputType input;
        /// <summary>
        /// 按键状态
        /// </summary>
        public bool active;
    }

    /// <summary>
    /// Gamepad/手柄
    /// </summary>
    public class Gamepad : Behavior
    {
        private Dictionary<InputType, InputInfo> InputMap = new()
        {
            { InputType.Joystick, new InputInfo { press = false, dire = TSVector2.zero } },
        };

        private Dictionary<InputType, bool> InputLockMap = new()
        {
            { InputType.Joystick, false },
        };

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<FPLateTickEvent>(OnFPLateTick);
            actor.stage.eventor.Listen<InputStateChangedEvent>(OnInputStateChanged);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<FPLateTickEvent>(OnFPLateTick);
            actor.stage.eventor.UnListen<InputStateChangedEvent>(OnInputStateChanged);
        }

        /// <summary>
        /// 获取手柄的某个按键状态
        /// </summary>
        /// <param name="inputType">按键类型</param>
        /// <returns>按键数据</returns>
        /// <exception cref="Exception">未找到该类型按键的数据</exception>
        public InputInfo GetInput(InputType inputType)
        {
            if (InputMap.TryGetValue(inputType, out InputInfo input)) return input;

            throw new Exception($"inputType not found {inputType}");
        }

        /// <summary>
        /// 设置按键数据
        /// </summary>
        /// <param name="inputType">按键类型</param>
        /// <param name="input">按键数据</param>
        public void SetInput(InputType inputType, InputInfo input)
        {
            if (InputLockMap[inputType]) return;

            if (InputMap.TryGetValue(inputType, out var oldInput))
            {
                // 上一次是 press，新的是 unpress 表示技能释放出去了。
                if (oldInput.press && false == input.press) input.release = true;
                InputMap.Remove(inputType);
            }

            InputMap.Add(inputType, input);
        }

        private void OnFPLateTick(FPLateTickEvent e)
        {
            ClearReleaseTokenAll();
        }

        /// <summary>
        /// 按键状态改变
        /// </summary>
        /// <param name="e"></param>
        private void OnInputStateChanged(InputStateChangedEvent e)
        {
            InputLockMap[e.input] = !e.active;
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
            InputMap.Remove(inputType);
            InputMap.Add(inputType, input);
        }
    }
}