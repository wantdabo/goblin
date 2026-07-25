using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Kowtow.Math;

namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// 外观组件 — FacadeInfo 的纯数据投影
/// </summary>
public sealed class FacadeComponent : Component
{
    /// <summary>
    /// 模型资源 ID
    /// </summary>
    public int model { get; set; }
    /// <summary>
    /// 动画推进模式
    /// </summary>
    public byte animticktype { get; set; }
    /// <summary>
    /// 动画状态
    /// </summary>
    public byte animstate { get; set; }
    /// <summary>
    /// 动画资源哈希
    /// </summary>
    public uint animhash { get; set; }
    /// <summary>
    /// 动画流逝时间
    /// </summary>
    public FP animelapsed { get; set; }
    /// <summary>
    /// 特效版本号
    /// </summary>
    public uint effincrement { get; set; }
    /// <summary>
    /// 待移除特效列表
    /// </summary>
    public List<uint> rmveffects { get; set; }
    /// <summary>
    /// 特效字典
    /// </summary>
    public Dictionary<uint, EffectInfo> effectdict { get; set; }
    /// <summary>
    /// 动画槽位列表
    /// </summary>
    public List<AnimationSlot> animslots { get; set; }

    /// <summary>
    /// 应用脏字段 [v1] — 后续迁至 SG 生成
    /// </summary>
    internal static void ApplyTo(object comp, ulong fieldmask, object[] values)
    {
        var c = (FacadeComponent)comp;
        var vi = 0;

        // Bit0: model
        if (0 != (fieldmask & 1)) c.model = (int)values[vi++];

        // Bit1: animticktype
        if (0 != (fieldmask & (1ul << 1))) c.animticktype = (byte)values[vi++];

        // Bit2: animstate
        if (0 != (fieldmask & (1ul << 2))) c.animstate = (byte)values[vi++];

        // Bit3: animhash
        if (0 != (fieldmask & (1ul << 3))) c.animhash = (uint)values[vi++];

        // Bit4: animelapsed
        if (0 != (fieldmask & (1ul << 4))) c.animelapsed = (FP)values[vi++];

        // Bit5: effectincrement
        if (0 != (fieldmask & (1ul << 5))) c.effincrement = (uint)values[vi++];

        // Bit6: rmveffects
        if (0 != (fieldmask & (1ul << 6))) c.rmveffects = (List<uint>)values[vi++];

        // Bit7: effectdict
        if (0 != (fieldmask & (1ul << 7))) c.effectdict = (Dictionary<uint, EffectInfo>)values[vi++];

        // Bit8: animslots
        if (0 != (fieldmask & (1ul << 8))) c.animslots = (List<AnimationSlot>)values[vi++];
    }
}
