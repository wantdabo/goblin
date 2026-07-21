using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL;

/// <summary>
/// 单层动画数据
/// </summary>
public struct LayerAnimEntry
{
    /// <summary>
    /// 动画层
    /// </summary>
    public byte layer { get; set; }
    /// <summary>
    /// 动画状态
    /// </summary>
    public byte animstate { get; set; }
    /// <summary>
    /// 动画名称哈希
    /// </summary>
    public uint animhash { get; set; }
    /// <summary>
    /// 该层独立流逝时间
    /// </summary>
    public uint elapsed { get; set; }
}

/// <summary>
/// 外观动画指令
/// </summary>
public class RIL_FACADE_ANIMATION : IRIL
{
    public override ushort id => RIL_DEFINE.FACADE_ANIMATION;
        
    /// <summary>
    /// 动画状态（layer 0 兼容字段）
    /// </summary>
    public byte animstate { get; set; }
    /// <summary>
    /// 动画名称哈希（layer 0 兼容字段）
    /// </summary>
    public uint animhash { get; set; }
    /// <summary>
    /// 流逝时间
    /// </summary>
    public uint animelapsed { get; set; }

    /// <summary>
    /// 多层动画数据（OnReady 时预分配，帧内零分配）
    /// </summary>
    public LayerAnimEntry[] layeranims { get; set; }
    /// <summary>
    /// 活跃层数
    /// </summary>
    public byte layercount { get; set; }
        
    protected override void OnReady()
    {
        animstate = 0;
        animhash = 0;
        animelapsed = 0;
        layeranims = new LayerAnimEntry[ANIM_DEFINE.LAYER_MAX];
        layercount = 0;
    }

    protected override void OnReset()
    {
        animstate = 0;
        animhash = 0;
        animelapsed = 0;
        for (byte i = 0; i < layercount; i++) layeranims[i] = default;
        layercount = 0;
    }
}