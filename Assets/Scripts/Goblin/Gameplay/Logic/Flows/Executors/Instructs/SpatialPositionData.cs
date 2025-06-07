using System;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// POSITION 变化指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class SpatialPositionData : InstructData
    {
        public override ushort id => INSTR_DEFINE.SPATIAL_POSITION;

        [ValueDropdown("@OdinValueDropdown.GetSpatialPositionDefine()", NumberOfItemsBeforeEnablingSearch = 0, DropdownTitle = "变化参考")]
        [PropertySpace(SpaceAfter = 5)]
        [LabelText("变化参考")]
        public byte type;
        
        [LabelText("X")]
        public int x;
        [LabelText("Y")]
        public int y;
        [LabelText("Z")]
        public int z;

        public override byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}