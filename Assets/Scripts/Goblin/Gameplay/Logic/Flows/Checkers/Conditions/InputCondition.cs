using System;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Defines;
using Kowtow.Math;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Checkers.Conditions
{
    /// <summary>
    /// 输入条件数据
    /// </summary>
    [Serializable]
    [InlineProperty]
    [MessagePackObject(true)]
    public class InputCondition : Condition
    {
        public override ushort id => CONDITION_DEFINE.INPUT;

        /// <summary>
        /// 输入类型
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetInputDefine()")]
        [LabelText("类型")]
        public ushort type = INPUT_DEFINE.JOYSTICK;
        
        /// <summary>
        /// 输入按下
        /// </summary>
        [OnValueChanged("OnPressChanged")]
        [LabelText("按下")]
        public bool press = true;
        private void OnPressChanged()
        {
            release = false == press;
        }
        
        /// <summary>
        /// 输入释放
        /// </summary>
        [LabelText("释放")]
        [OnValueChanged("OnReleaseChanged")]
        public bool release;
        private void OnReleaseChanged()
        {
            press = false == release;
        }
    }
}