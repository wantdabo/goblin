using System;
using System.Collections.Generic;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Common
{
    /// <summary>
    /// 管线数据读取
    /// </summary>
    public static class PipelineDataReader
    {
        /// <summary>
        /// 缓存, 管线数据字典, 键为管线 ID, 值为管线数据
        /// </summary>
        private static Dictionary<uint, PipelineData> datas { get; set; } = new();

        /// <summary>
        /// 加载管线数据
        /// </summary>
        /// <param name="id">管线 ID</param>
        /// <returns>管线数据</returns>
        public static PipelineData ReadPipelineData(uint id)
        {
            if (datas.TryGetValue(id, out var data)) return data;

            // TODO 后续加宏, 判断非 UNITY 环境使用 File.ReadAllBytes
            var bytes = Export.engine.gameres.location.LoadPipelineSync(id.ToString());
            
            data = MessagePackSerializer.Deserialize<PipelineData>(bytes);
            datas.Add(id, data);

            return data;
        }
        
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
    }
}