using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 状态变更指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class ChangeStateData : InstructData
    {
        public override ushort id => INSTR_DEFINE.CHANGE_STATE;

        /// <summary>
        /// 是否打断当前状态
        /// </summary>
        [LabelText("是否打断当前状态")]
        public bool breakable;

        /// <summary>
        /// 是否强制变更, 如果为真, 则忽略状态切换规则直接变更
        /// </summary>
        [ShowIf("@false == breakable")]
        [LabelText("是否强制变更, 如果为真, 则忽略状态切换规则直接变更")]
        public bool force;
        
        /// <summary>
        /// 变更的状态
        /// </summary>
        [ShowIf("@false == breakable")]
        [LabelText("变更的状态")]
        [ValueDropdown("@OdinValueDropdown.GetStateDefine()")]
        public byte state;

        /// <summary>
        /// 是否使用延迟中断状态
        /// </summary>
        [LabelText("是否使用延迟中断状态")]
        public bool usedelaybreak;

        /// <summary>
        /// 延迟中断时间
        /// </summary>
        [ShowIf("@usedelaybreak")]
        [LabelText("延迟中断时间")]
        public uint delaybreak = 1000;
    }
}