using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 生成魔法体指令数据
/// </summary>
[MessagePackObject(true)]
public class CreateMagicData : InstructData
{
    public override ushort id => INSTR_DEFINE.CREATE_MAGIC;

    /// <summary>
    /// 生成原点类型
    /// </summary>
    public byte origin;
    /// <summary>
    /// 生成原点偏移
    /// </summary>
    public IntVector3 offset;
    /// <summary>
    /// 生成初始旋转类型
    /// </summary>
    public byte euler;
    /// <summary>
    /// 生成旋转角度
    /// </summary>
    public int angle;
    /// <summary>
    /// 缩放
    /// </summary>
    public int scale;
    /// <summary>
    /// 管线列表
    /// </summary>
    public GBLList<uint> pipelines;

    public CreateMagicData()
    {
        et = FLOW_DEFINE.ET_FLOW_OWNER;
    }
}
