using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows
{
    /// <summary>
    /// 管线原始数据格式
    /// 这个才能支持 MessagePack 序列化, 它的职责也仅仅是为了持久化, 与 PipelineData 等价的
    /// </summary>
    [MessagePackObject(true)]
    public class PipelineRawData
    {
        /// <summary>
        /// 管线长度
        /// </summary>
        public ulong length { get; set; }
        /// <summary>
        /// 区间开始
        /// </summary>
        public ulong[] begin { get; set; }
        /// <summary>
        /// 区间结束
        /// </summary>
        public ulong[] end { get; set; }
        /// <summary>
        /// 是否只检查一次
        /// </summary>
        public bool[] checkonce { get; set; }
        /// <summary>
        /// 指令类型
        /// </summary>
        public ushort[] instrtypes { get; set; }
        /// <summary>
        /// 指令数据
        /// </summary>
        public byte[][] instrdata { get; set; }
        /// <summary>
        /// 条件类型
        /// </summary>
        public ushort[][] conditiontypes { get; set; }
        /// <summary>
        /// 条件数据
        /// </summary>
        public byte[][][] conditions { get; set; }
        /// <summary>
        /// 火花触发范围
        /// </summary>
        public sbyte[] sparkinfluences { get; set; }
        /// <summary>
        /// 火花令牌
        /// </summary>
        public string[] sparktoken { get; set; }
        /// <summary>
        /// 火花令牌变体
        /// </summary>
        public string[] sparktokenvariant { get; set; }
        /// <summary>
        /// 火花指令类型
        /// </summary>
        public ushort[] sparkinstrtypes { get; set; }
        /// <summary>
        /// 火花指令数据
        /// </summary>
        public byte[][] sparkinstrdata { get; set; }
        /// <summary>
        /// 火花条件类型
        /// </summary>
        public ushort[][] sparkconditiontypes { get; set; }
        /// <summary>
        /// 火花条件数据
        /// </summary>
        public byte[][][] sparkconditions { get; set; }
    }
}