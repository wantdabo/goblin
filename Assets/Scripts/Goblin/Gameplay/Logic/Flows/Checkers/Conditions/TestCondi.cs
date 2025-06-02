using System;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Defines;
using Kowtow.Math;
using MessagePack;
using Sirenix.OdinInspector;

namespace Goblin.Gameplay.Logic.Flows.Checkers.Conditions
{
    [Serializable]
    [InlineProperty]
    [MessagePackObject(true)]
    public class TestCondi : Condition
    {
        public override ushort id => CONDITION_DEFINE.TEST;

        [LabelText("时间缩放")]
        public uint timescale;
        
        public override byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}