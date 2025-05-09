using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 按键输入枚举
    /// </summary>
    public enum InputType
    {
        /// <summary>
        /// 摇杆
        /// </summary>
        Joystick = 1,
        /// <summary>
        /// 按钮 A
        /// </summary>
        BA = 2,
        /// <summary>
        /// 按钮 B
        /// </summary>
        BB = 3,
        /// <summary>
        /// 按钮 C
        /// </summary>
        BC = 4,
        /// <summary>
        /// 按钮 D
        /// </summary>
        BD = 5,
    }

    /// <summary>
    /// 按键输入的结构体
    /// </summary>
    public struct InputInfo
    {
        /// <summary>
        /// 摁下之后 -> TRUE
        /// </summary>
        public bool press { get; set; }
        /// <summary>
        /// 摁下之后，抬起 -> TRUE
        /// </summary>
        public bool release { get; set; }
        /// <summary>
        /// 按键的方向
        /// </summary>
        public FPVector2 dire { get; set; }
    }
    
    /// <summary>
    /// 手柄信息
    /// </summary>
    public class GamepadInfo : BehaviorInfo
    {
        /// <summary>
        /// 输入数据集合
        /// </summary>
        public Dictionary<InputType, InputInfo> inputdict { get; set; }

        protected override void OnReady()
        {
            inputdict = ObjectCache.Get<Dictionary<InputType, InputInfo>>();
            inputdict.Add(InputType.Joystick, new InputInfo { press = false, release = false, dire = FPVector2.zero });
            inputdict.Add(InputType.BA, new InputInfo { press = false, release = false, dire = FPVector2.zero });
            inputdict.Add(InputType.BB, new InputInfo { press = false, release = false, dire = FPVector2.zero });
            inputdict.Add(InputType.BC, new InputInfo { press = false, release = false, dire = FPVector2.zero });
            inputdict.Add(InputType.BD, new InputInfo { press = false, release = false, dire = FPVector2.zero });
        }

        protected override void OnReset()
        {
            inputdict.Clear();
            ObjectCache.Set(inputdict);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<GamepadInfo>();
            clone.Ready(id);
            foreach (var kv in inputdict)
            {
                clone.inputdict.Remove(kv.Key);
                clone.inputdict.Add(kv.Key, kv.Value);
            }
            
            return clone;
        }
    }
}