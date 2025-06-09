using System;
using Kowtow.Math;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

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

        [LabelText("POSITION 变化数据")]
        public IntVector3 position;

        public override byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}