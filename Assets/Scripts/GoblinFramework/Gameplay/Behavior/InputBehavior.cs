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
        public Fixed64Vector2 dire;
    }

    public class InputBehavior : Behavior<InputBehavior.InputInfo>
    {
        public enum InputType
        {
            LJoystick,
            LBA,
            LBB,
            LBC,
            LBD,
        }

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
            if (Info.InputMap.ContainsKey(inputType)) Info.InputMap.Remove(inputType);

            Info.InputMap.Add(inputType, input);
        }

        #region InputInfo
        public class InputInfo : LInfo
        {
            public Dictionary<InputType, Input> InputMap = new Dictionary<InputType, Input>
            {
                {InputType.LJoystick, new Input(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.LBA, new Input(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.LBB, new Input(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.LBC, new Input(){press = false, dire = Fixed64Vector2.Zero}},
                {InputType.LBD, new Input(){press = false, dire = Fixed64Vector2.Zero}},
            };

            public override object Clone()
            {
                var inputInfo = new InputInfo();
                foreach (var kv in InputMap)
                {
                    inputInfo.InputMap.Remove(kv.Key);
                    inputInfo.InputMap.Add(kv.Key, kv.Value);
                }

                return inputInfo;
            }
        }
        #endregion
    }
}
