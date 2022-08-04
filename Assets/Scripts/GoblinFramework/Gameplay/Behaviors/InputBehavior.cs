using GoblinFramework.Core;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public struct Input
    {
        public bool press;
        public bool release;
        public Fixed64Vector2 dire;
    }

    public enum InputType
    {
        Joystick,
        BA,
        BB,
        BC,
        BD,
    }

    public class InputBehavior : Behavior<InputBehavior.InputInfo>
    {
        /// <summary>
        /// 获取输入状态
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>输入状态</returns>
        /// <exception cref="Exception">未找到输入类型异常</exception>
        public Input GetInput(InputType inputType)
        {
            if (Info.InputMap.TryGetValue(inputType, out Input input)) return input;

            throw new Exception($"inputType not found {inputType}");
        }

        /// <summary>
        /// 设置输入状态
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="input">输入状态</param>
        public void SetInput(InputType inputType, Input input)
        {

            if (Info.InputMap.TryGetValue(inputType, out var oldInput))
            {
                // 上一次是 Press，新的是 UnPress 表示技能释放出去了。
                if (oldInput.press && false == input.press) input.release = true;
                Info.InputMap.Remove(inputType);
            }

            Info.InputMap.Add(inputType, input);
        }

        #region InputInfo
        public class InputInfo : BehaviorInfo
        {
            public Dictionary<InputType, Input> InputMap = new Dictionary<InputType, Input>
            {
                {InputType.Joystick, new Input(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.BA, new Input(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.BB, new Input(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.BC, new Input(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.BD, new Input(){press = false, dire = Fixed64Vector2.Zero}},
            };
        }
        #endregion
    }
}
