using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Scriptings;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;

namespace Goblin.Gameplay.Logic.Flows
{
    /// <summary>
    /// 管线数据读取
    /// </summary>
    public static class PipelineDataReader
    {
        /// <summary>
        /// 管线数据字典, 键为管线 ID, 值为管线数据
        /// </summary>
        private static Dictionary<uint, PipelineData> datas { get; set; } = new();
        /// <summary>
        /// 脚本数据字典, 键为脚本 ID, 值为脚本处理器
        /// </summary>
        private static Dictionary<uint, Scripting> scriptings { get; set; } = new();

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static PipelineDataReader()
        {
            Scriptings();
        }

        /// <summary>
        /// 加载管线数据
        /// </summary>
        /// <param name="id">管线 ID</param>
        /// <returns>管线数据</returns>
        public static PipelineData Read(uint id)
        {
            if (datas.TryGetValue(id, out var data)) return data;

            // 从脚本处理器中尝试获取 PipelineData
            if (false == scriptings.TryGetValue(id, out var scripting)) throw new Exception("pipeline data not found.");
            
            data = scripting.Script();
            datas.Add(id, data);

            return data;
        }
        
        /// <summary>
        /// 预载入管线数据
        /// </summary>
        /// <param name="id">管线 ID</param>
        /// <param name="bytes">管线二进制数据</param>
        public static void PreloadPipelineData(uint id, byte[] bytes)
        {
            if (datas.ContainsKey(id)) return;

            // 反序列化管线数据
            var data = MessagePack.MessagePackSerializer.Deserialize<PipelineRawData>(bytes);
            datas.Add(id, data.ToPipelineData());
        }

        /// <summary>
        /// 格式化管线数据
        /// </summary>
        /// <param name="data">管线数据</param>
        public static void Format(this PipelineData data)
        {
            // 求出最大的管线长度
            ulong length = 0;
            foreach (var instruct in data.instructs)
            {
                if (length >= instruct.end) continue;
                length = instruct.end;
            }
            data.length = length;
            data.instructs.Sort((a, b) => a.begin.CompareTo(b.begin));
        }

        /// <summary>
        /// 查询指令
        /// </summary>
        /// <param name="data">管线数据</param>
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
                if (instruct.begin >= timeline) break;
                if (null == instrinfos) instrinfos = ObjectCache.Ensure<List<(bool inside, uint index, Instruct instruct)>>();
                instrinfos.Add((instruct.begin <= timeline && instruct.end > timeline, index, instruct));
            }

            return null != instrinfos && instrinfos.Count > 0;
        }

        /// <summary>
        /// 查询指令
        /// </summary>
        /// <param name="data">管道数据</param>
        /// <param name="index">指令索引</param>
        /// <param name="instruct">指令</param>
        /// <returns>YES/NO</returns>
        public static bool Query(this PipelineData data, uint index, out Instruct instruct)
        {
            instruct = default;
            if (index < 1 || index > data.instructs.Count) return false;

            instruct = data.instructs[(int)(index - 1)];
            return true;
        }

        /// <summary>
        /// 初始化脚本处理器
        /// </summary>
        private static void Scriptings()
        {
            void Scripting<T>() where T : Scripting, new()
            {
                var scripting = new T();
                scriptings.Add(scripting.id, scripting);
            }

            Scripting<S100000001>();
            Scripting<S100000002>();
        }
    }
}