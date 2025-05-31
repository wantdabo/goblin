using System.Collections.Generic;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows
{
    /// <summary>
    /// 管线数据
    /// </summary>
    [MessagePackObject(true)]
    public class PipelineData
    {
        /// <summary>
        /// 模型
        /// </summary>
        public int model { get; set; }
        /// <summary>
        /// 管线长度, 根据指令列表中区间结束的最大值来计算得出
        /// </summary>
        public ulong length { get; set; }
        /// <summary>
        /// 指令列表
        /// </summary>
        public List<Instruct> instructs { get; set; }
    }
}