﻿using GoblinFramework.Core;
using GoblinFramework.Logic.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Logic.Gameplay
{
    public struct Input
    {
        public bool press;
        public bool release;
        public TSVector2 dire;
    }

    public enum InputType
    {
        Joystick,
        BA,
        BB,
        BX,
        BY,
    }

    /// <summary>
    /// InputBehavior，输入行为组件
    /// </summary>
    public class InputBehavior : Behavior<InputBehavior.InputInfo>, ILateLoop
    {
        private List<InputType> totalInputKeys = new List<InputType>();

        protected override void OnCreate()
        {
            base.OnCreate();
            foreach (var name in Enum.GetNames(typeof(InputType))) if (Enum.TryParse(name, out InputType result)) totalInputKeys.Add(result);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            totalInputKeys.Clear();
        }

        /// <summary>
        /// 获取输入状态
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <returns>输入状态</returns>
        /// <exception cref="Exception">未找到输入类型异常</exception>
        public Input GetInput(InputType inputType)
        {
            if (info.InputMap.TryGetValue(inputType, out Input input)) return input;

            throw new Exception($"inputType not found {inputType}");
        }

        /// <summary>
        /// 设置输入状态
        /// </summary>
        /// <param name="inputType">输入类型</param>
        /// <param name="input">输入状态</param>
        public void SetInput(InputType inputType, Input input)
        {

            if (info.InputMap.TryGetValue(inputType, out var oldInput))
            {
                // 上一次是 Press，新的是 UnPress 表示技能释放出去了。
                if (oldInput.press && false == input.press) input.release = true;
                info.InputMap.Remove(inputType);
            }

            info.InputMap.Add(inputType, input);
        }

        /// <summary>
        /// 任意输入
        /// </summary>
        /// <returns>任意键，true，否则 false</returns>
        public bool HasAnyInput()
        {
            foreach (var kv in info.InputMap) if (kv.Value.press || kv.Value.release) return true;

            return false;
        }

        public void PLateLoop(int frame, FP detailTime)
        {
            // 清理 Release 状态
            foreach (var key in totalInputKeys)
            {
                info.InputMap.TryGetValue(key, out var input);
                info.InputMap.Remove(key);
                input.release = false;
                info.InputMap.Add(key, input);
            }
        }

        #region InputInfo
        public class InputInfo : BehaviorInfo
        {
            public Dictionary<InputType, Input> InputMap = new Dictionary<InputType, Input>
            {
                {InputType.Joystick, new Input(){press = false, dire = TSVector2.zero}},
                {InputType.BA, new Input(){press = false, dire = TSVector2.zero}},
                {InputType.BB, new Input(){press = false, dire = TSVector2.zero}},
                {InputType.BX, new Input(){press = false, dire = TSVector2.zero}},
                {InputType.BY, new Input(){press = false, dire = TSVector2.zero}},
            };
        }
        #endregion
    }
}