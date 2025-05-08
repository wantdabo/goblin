using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;

namespace Goblin.Gameplay.Logic.Flows.Common
{
    /// <summary>
    /// 管道数据读取
    /// </summary>
    public static class PipelineDataReader
    {
        /// <summary>
        /// 插入指令
        /// </summary>
        /// <param name="data">管道数据</param>
        /// <param name="instruct">指令</param>
        public static void Insert(this PipelineData data, Instruct instruct)
        {
            data.instructs.Add(instruct);
            data.length = Math.Max(data.length, instruct.end);
        }

        /// <summary>
        /// 查询指令
        /// </summary>
        /// <param name="data">管道数据</param>
        /// <param name="timeline">时间线</param>
        /// <param name="instructs">处于时间线的指令列表, (index : 指令索引, instruct : 指令)</param>
        /// <returns>YES/NO</returns>
        public static bool Query(PipelineData data, ulong timeline, out List<(uint index, Instruct instruct)> instructs)
        {
            instructs = default;
            uint index = 0;
            foreach (var instruct in data.instructs)
            {
                index++;
                if (instruct.begin > timeline) break;
                if (instruct.end < timeline) continue;
                if (instruct.begin >= timeline && instruct.end >= timeline)
                {
                    if (null == instructs) instructs = ObjectCache.Get<List<(uint index, Instruct instruct)>>();
                    instructs.Add((index, instruct));
                }
            }
            
            return null != instructs && instructs.Count > 0;
        }
        
        /// <summary>
        /// 排序指令
        /// </summary>
        /// <param name="data">管道数据</param>
        public static void Sort(this PipelineData data)
        {
            data.instructs.Sort((x, y) =>
            {
                if (x.begin == y.begin) return 0;
                
                return x.begin < y.begin ? -1 : 1;
            });
        }
    }
}