using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Flows;

/// <summary>
/// 管线信息
/// </summary>
public partial class FlowInfo : BehaviorInfo
{
    /// <summary>
    /// 管线的拥有者
    /// </summary>
    public ulong owner { get; set; }
    /// <summary>
    /// 管线的时间长度, 根据管线 ID 列表中区间结束的最大值来计算得出
    /// </summary>
    public ulong length { get; set; }
    /// <summary>
    /// 管线的时间线
    /// </summary>
    public ulong timeline { get; set; }
    /// <summary>
    /// 管线的经过时间, 满足单帧才能执行, 如果溢出, 以此循环执行
    /// </summary>
    public ulong framepass { get; set; }
    /// <summary>
    /// 管线的 ID 列表, 用于指向管线数据
    /// </summary>
    public List<uint> pipelines { get; set; }
    /// <summary>
    /// 管线的执行中 ID 集合, 用于触发管线生命周期
    /// </summary>
    public Dictionary<uint, List<uint>> doings { get; set; }
    /// <summary>
    /// 各管线已完成的指令索引（pipelineid → 最后一条 end ＜ timeline 的 index）
    /// 用于 RunPipeline 跳过已过期指令，避免每帧全量扫描
    /// </summary>
    public Dictionary<uint, uint> completedindex { get; set; }

    protected override void OnReady()
    {
        // FlowInfo 默认不激活，由 Flow 显式激活
        active = false;
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == pipelines) pipelines = ObjectCache.Ensure<List<uint>>();
        if (null == doings) doings = ObjectCache.Ensure<Dictionary<uint, List<uint>>>();
        if (null == completedindex) completedindex = ObjectCache.Ensure<Dictionary<uint, uint>>();
    }
}
