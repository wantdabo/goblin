using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 碰撞指令数据
    /// </summary>
    [MessagePackObject(true)]
    public class CollisionData : InstructData
    {
        public override ushort id => INSTR_DEFINE.COLLISION;

        /// <summary>
        /// 碰撞类型
        /// </summary>
        public byte type;
        /// <summary>
        /// 偏移
        /// </summary>
        public IntVector3 offset;
        
        /// <summary>
        /// 射线方向
        /// </summary>
        public IntVector3 raydire;
        /// <summary>
        /// 射线长度
        /// </summary>
        public uint raydis;

        /// <summary>
        /// 线段终点
        /// </summary>
        public IntVector3 lineep;

        /// <summary>
        /// 立方体大小
        /// </summary>
        public IntVector3 boxsize;
        
        /// <summary>
        /// 球体半径
        /// </summary>
        public uint sphereradius;
        
        public override byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
    }
}