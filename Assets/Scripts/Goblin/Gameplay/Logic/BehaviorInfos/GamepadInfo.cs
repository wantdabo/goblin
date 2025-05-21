using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
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
        public Dictionary<ushort, InputInfo> inputdict { get; set; }

        protected override void OnReady()
        {
            inputdict = ObjectCache.Ensure<Dictionary<ushort, InputInfo>>();
            inputdict.Add(INPUT_DEFINE.JOYSTICK, new InputInfo { press = false, release = false, dire = FPVector2.zero });
            inputdict.Add(INPUT_DEFINE.BA, new InputInfo { press = false, release = false, dire = FPVector2.zero });
            inputdict.Add(INPUT_DEFINE.BB, new InputInfo { press = false, release = false, dire = FPVector2.zero });
            inputdict.Add(INPUT_DEFINE.BC, new InputInfo { press = false, release = false, dire = FPVector2.zero });
            inputdict.Add(INPUT_DEFINE.BD, new InputInfo { press = false, release = false, dire = FPVector2.zero });
        }

        protected override void OnReset()
        {
            inputdict.Clear();
            ObjectCache.Set(inputdict);
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Ensure<GamepadInfo>();
            clone.Ready(id);
            foreach (var kv in inputdict)
            {
                clone.inputdict.Remove(kv.Key);
                clone.inputdict.Add(kv.Key, kv.Value);
            }
            
            return clone;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 31 + id.GetHashCode();
            foreach (var kv in inputdict)
            {
                hash = hash * 31 + kv.Key.GetHashCode();
                hash = hash * 31 + kv.Value.press.GetHashCode();
                hash = hash * 31 + kv.Value.release.GetHashCode();
                hash = hash * 31 + kv.Value.dire.GetHashCode();
            }
            
            return hash;
        }
    }
}