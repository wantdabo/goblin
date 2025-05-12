using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Flows.Common
{
    /// <summary>
    /// 管线数据写入
    /// </summary>
    public class PipelineDataWriter
    {
        /// <summary>
        /// 加载管线数据
        /// </summary>
        /// <param name="id">管线 ID</param>
        /// <param name="instructs">指令列表</param>
        public static void WritePipelineData(uint id, List<Instruct> instructs)
        {
            // 求出最大的管线长度
            ulong length = 0;
            foreach (var instruct in instructs)
            {
                if (length >= instruct.end) continue;
                length = instruct.end;
            }
            
            PipelineData data = new PipelineData();
            data.id = id;
            data.instructs = instructs;
            data.instructs.Sort((a, b) => a.begin.CompareTo(b.begin));
        }
    }
}