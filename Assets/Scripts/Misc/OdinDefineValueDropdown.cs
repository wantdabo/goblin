using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Sirenix.OdinInspector;

namespace Goblin.Misc
{
    /// <summary>
    /// Odin 定义值下拉列表
    /// </summary>
    public class OdinDefineValueDropdown
    {
        /// <summary>
        /// 获取状态定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(状态)</returns>
        public static ValueDropdownList<byte> GetStateDefine()
        {
            return new ()
            {
                { "待机", STATE_DEFINE.IDLE },
                { "移动", STATE_DEFINE.MOVE },
                { "跳跃", STATE_DEFINE.JUMP },
                { "下坠", STATE_DEFINE.FALL },
                { "技能", STATE_DEFINE.CASTING },
            };
        }

        /// <summary>
        /// 获取条件定义下拉列表
        /// </summary>
        /// <returns>ValueDropdownList(条件 ID)</returns>
        public static ValueDropdownList<ushort> GetConditionDefine()
        {
            return new ()
            {
                { "测试指令", CONDITION_DEFINE.TEST },
            };
        }
    }
}