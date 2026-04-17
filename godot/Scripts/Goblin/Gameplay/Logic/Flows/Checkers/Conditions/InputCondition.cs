using System;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Defines;
using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Checkers.Conditions
{
    /// <summary>
    /// 输入条件数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class InputCondition : Condition
    {
        public override ushort id => CONDITION_DEFINE.INPUT;

        /// <summary>
        /// 输入类型
        /// </summary>
        public ushort type = INPUT_DEFINE.JOYSTICK;
        
        /// <summary>
        /// 输入按下
        /// </summary>
        public bool press = true;
        private void OnPressChanged()
        {
            release = false == press;
        }
        
        /// <summary>
        /// 输入释放
        /// </summary>
        public bool release;
        private void OnReleaseChanged()
        {
            press = false == release;
        }
    }
}