using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 创建子弹数据
    /// </summary>
    [MessagePackObject(true)]
    public class CreateBulletData : InstructData
    {
        public override ushort id => INSTR_DEFINE.CREATE_BULLET;
        
        /// <summary>
        /// 子弹的伤害强度
        /// </summary>
        public uint strength { get; set; }
        /// <summary>
        /// 子弹的速度
        /// </summary>
        public uint speed { get; set; }
        /// <summary>
        /// 子弹生成原点类型
        /// </summary>
        public byte origin { get; set; }
        /// <summary>
        /// 子弹生成原点偏移
        /// </summary>
        public GPVector3 offset { get; set; }
        /// <summary>
        /// 子弹生成初始旋转类型
        /// </summary>
        public byte euler { get; set; }
        /// <summary>
        /// 子弹生成旋转角度
        /// </summary>
        public FP angle { get; set; }
        /// <summary>
        /// 缩放
        /// </summary>
        public int scale { get; set; }
        /// <summary>
        /// 管线列表
        /// </summary>
        public List<uint> pipelines { get; set; }
    }
}