using System.Collections.Generic;
using Goblin.Gameplay.Flows.Executors.Common;

namespace Goblin.Gameplay.Flows
{
    /// <summary>
    /// 管线数据
    /// </summary>
    public class PipelineData
    {
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