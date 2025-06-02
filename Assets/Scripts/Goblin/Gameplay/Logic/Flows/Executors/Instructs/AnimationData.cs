using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 动画指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class AnimationData : InstructData
    {
        public override ushort id => INSTR_DEFINE.ANIMATION;

        public string name;
        
        public override byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}