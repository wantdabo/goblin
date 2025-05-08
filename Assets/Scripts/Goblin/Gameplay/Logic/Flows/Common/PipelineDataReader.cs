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
        /// 查询指令
        /// </summary>
        /// <param name="data">管道数据</param>
        /// <param name="timeline">时间线</param>
        /// <param name="instrinfos">处于时间线的指令信息列表, (index : 指令索引, instruct : 指令)</param>
        /// <returns>YES/NO</returns>
        public static bool Query(this PipelineData data, ulong timeline, out List<(bool inside, uint index, Instruct instruct)> instrinfos)
        {
            instrinfos = default;
            uint index = 0;
            foreach (var instruct in data.instructs)
            {
                index++;
                if (instruct.begin > timeline) break;
                if (null == instrinfos) instrinfos = ObjectCache.Get<List<(bool inside, uint index, Instruct instruct)>>();
                instrinfos.Add((instruct.begin >= timeline && instruct.end >= timeline, index, instruct));
            }
            
            return null != instrinfos && instrinfos.Count > 0;
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