using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 释放技能指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class LaunchSkillData : InstructData
    {
        public override ushort id => INSTR_DEFINE.LAUNCH_SKILL;

        /// <summary>
        /// 技能 ID
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.SKillIds()")]
        [LabelText("技能 ID")]
        public uint skillid;
        
        public override byte[] Serialize()
        {
           return MessagePackSerializer.Serialize(this);
        }
    }
}