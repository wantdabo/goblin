using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Common;

namespace Goblin.Gameplay.Logic.Flows;

/// <summary>
/// 管线数据
/// </summary>
public sealed class PipelineData
{
    /// <summary>
    /// 管线长度, 根据指令列表中区间结束的最大值来计算得出
    /// </summary>
    public ulong length { get; set; }
    /// <summary>
    /// 指令列表
    /// </summary>
    public GBLList<Instruct> instructs { get; set; }
    /// <summary>
    /// 火花指令列表
    /// </summary>
    public GBLList<SparkInstruct> sparkinstructs { get; set; }
    /// <summary>
    /// 管线索引（管线可能包含多个 PipelineData，需要索引区分）
    /// </summary>
    public uint index { get; set; }

    /// <summary>
    /// 格式化管线数据
    /// </summary>
    public void Format()
    {
        instructs.Sort((a, b) => a.begin.CompareTo(b.begin));
    }
}
