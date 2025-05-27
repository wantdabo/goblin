using System.Collections.Generic;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Scriptings;
using Goblin.Gameplay.Logic.Flows.Scriptings.Common;
using MessagePack;

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
            if (scriptings.TryGetValue(id, out var scripting))
            {
                data = scripting.Script();
                datas.Add(id, data);

                return data;
            }

            // TODO 后续加宏, 判断非 UNITY 环境使用 File.ReadAllBytes
            var bytes = Export.engine.gameres.location.LoadPipelineSync(id.ToString());

            data = MessagePackSerializer.Deserialize<PipelineData>(bytes);
            datas.Add(id, data);

            return data;
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
                if (null == instrinfos) instrinfos = ObjectCache.Ensure<List<(bool inside, uint index, Instruct instruct)>>();
                instrinfos.Add((instruct.begin <= timeline && instruct.end >= timeline, index, instruct));
            }

            return null != instrinfos && instrinfos.Count > 0;
        }

        /// <summary>
        /// 初始化脚本处理器
        /// </summary>
        private static void Scriptings()
        {
            scriptings.Add(FLOW_DEFINE.S100000001, new S100000001());
            scriptings.Add(FLOW_DEFINE.S100000002, new S100000002());
        }
    }
}