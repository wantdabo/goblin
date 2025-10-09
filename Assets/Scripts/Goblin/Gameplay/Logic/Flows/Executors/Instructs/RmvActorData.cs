using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 移除 Actor 指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class RmvActorData : InstructData
    {
        public override ushort id => INSTR_DEFINE.RMV_ACTOR;
        
        /// <summary>
        /// 移除的目标
        /// </summary>
        [ValueDropdown("@OdinValueDropdown.GetRmvActorDefine()")]
        [LabelText("移除的目标")]
        public byte target;
        
        public override byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}